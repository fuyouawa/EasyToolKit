using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Utility class for discovering and matching inspector elements that implement <see cref="IHandler"/>.
    /// Elements are discovered via reflection, sorted by priority obtained from <see cref="IPriorityAccessor"/> attributes,
    /// and registered in a <see cref="s_typeMatcher"/> for type-based matching.
    /// </summary>
    public static class HandlerUtility
    {
        private static Type[] s_elementTypes;
        private static TypeMatcher s_typeMatcher;
        private static bool s_typeMatcherInitialized;
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Gets or sets a callback that provides a default <see cref="Priority"/> when no priority attribute is found.
        /// If not set, returns null when no priority attribute is present.
        /// </summary>
        private static readonly List<Func<Type, Priority>> NullPriorityFallbacks = new List<Func<Type, Priority>>();

        public static TypeMatcher TypeMatcher
        {
            get
            {
                EnsureTypeMatcherInitialized();
                return s_typeMatcher;
            }
        }

        /// <summary>
        /// Adds a fallback function that provides a default priority when no priority attribute is found.
        /// This will reset the type matcher to ensure newly added elements are sorted with the updated fallback.
        /// </summary>
        /// <param name="fallback">The fallback function to add.</param>
        public static void AddNullPriorityFallback(Func<Type, Priority> fallback)
        {
            NullPriorityFallbacks.Add(fallback);
            lock (InitializationLock)
            {
                s_typeMatcherInitialized = false;
                s_typeMatcher = null;
            }
        }

        private static void EnsureTypeMatcherInitialized()
        {
            if (s_typeMatcherInitialized) return;
            lock (InitializationLock)
            {
                if (s_typeMatcherInitialized) return;
                InitializeTypeMatcher();
                s_typeMatcherInitialized = true;
            }
        }

        private static void InitializeTypeMatcher()
        {
            if (s_elementTypes == null)
            {
                var elementTypes = new List<Type>();

                foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(asm => asm.GetTypes())
                             .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract))
                {
                    if (type.IsInheritsFrom<IHandler>())
                    {
                        elementTypes.Add(type);
                    }
                }
                s_elementTypes = elementTypes.ToArray();
            }

            s_typeMatcher = new TypeMatcher();
            s_typeMatcher.SetTypeMatchIndices(s_elementTypes
                .OrderByDescending(GetElementPriority)
                .Select((type, i) =>
                {
                    var index = new TypeMatchIndex(type, s_elementTypes.Length - i, null);
                    if (type.BaseType != null && type.BaseType.IsGenericType)
                    {
                        // For generic inspector elements, extract the target generic type from the inheritance chain.
                        // Example: For `SerializedDictionaryValueDrawer<TValue> : EasyValueDrawer<SerializedDictionary<string, TValue>>`,
                        // `OpenGenericType` is `EasyValueDrawer<>` (the base class), and
                        // `GetArgumentsOfInheritedOpenGenericType` returns `SerializedDictionary<string, TValue>`,
                        // which is stored in `index.Targets` for later matching in `GetMatchedType`.
                        index.Targets = type.GetArgumentsOfInheritedOpenGenericType(type.BaseType.GetGenericTypeDefinition());
                    }

                    return index;
                }));

            s_typeMatcher.AddMatchRule(GetMatchedType);
        }

        /// <summary>
        /// Gets element types for the specified inspector element.
        /// Matches elements against the element's definition and properties.
        /// </summary>
        /// <param name="element">The inspector element to match.</param>
        /// <returns>An enumerable of types, ordered by priority.</returns>
        public static IEnumerable<Type> GetElementTypes(IElement element, Func<Type, bool> typeFilter = null, IList<Type[]> additionalMatchTypesList = null)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetMatches(Type.EmptyTypes),
            };

            // If the element is a value element, use its value type for matching
          if (element is IValueElement valueElement)
          {
              resultsList.Add(TypeMatcher.GetMatches(valueElement.ValueEntry.ValueType));
          }

            if (additionalMatchTypesList != null)
            {
                foreach (var matchTypes in additionalMatchTypesList)
                {
                    resultsList.Add(TypeMatcher.GetMatches(matchTypes));
                }
            }

            var typeSet = new HashSet<Type>();
            var results = TypeMatcher.GetMergedResults(resultsList);
            foreach (var result in results)
            {
                var type = result.MatchedType;
                if (!typeSet.Add(type))
                {
                    continue;
                }

                if (typeFilter != null)
                {
                    if (!typeFilter(type))
                    {
                        continue;
                    }
                }

                if (!CanHandleElement(type, element))
                {
                    continue;
                }

                yield return type;
            }
        }

        /// <summary>
        /// Resolves generic type arguments for inspector elements that target partially open generic types.
        /// This method handles complex scenarios where an inspector element's generic parameters correspond to
        /// specific type parameters within a partially concrete generic target type.
        /// </summary>
        /// <param name="matchIndex">The type match index containing the inspector element type and its target generic type.</param>
        /// <param name="targets">The target types to match against; expected to contain exactly one type (the actual value type).</param>
        /// <param name="stopMatch">Reference parameter that can be set to true to stop further matching (not used in this implementation).</param>
        /// <returns>
        /// If the inspector element's target type is a (partially open) generic type and the actual value type
        /// can provide the missing generic arguments, returns the concretely instantiated inspector element type.
        /// Otherwise, returns null to indicate no match.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method enables inspector elements to target complex generic types with mixed concrete and generic type parameters.
        /// For example, consider an inspector element type that draws values of a generic collection with a fixed key type:
        /// </para>
        /// <code>
        /// class SerializedDictionaryValueDrawer&lt;TValue&gt; : EasyValueDrawer&lt;SerializedDictionary&lt;string, TValue&gt;&gt;
        /// </code>
        /// <para>
        /// Here, <c>SerializedDictionaryValueDrawer&lt;TValue&gt;</c> (matchIndex.Type) targets
        /// <c>SerializedDictionary&lt;string, TValue&gt;</c> (matchIndex.Targets[0]),
        /// which is a partially open generic type with a concrete key type (string) and a generic value type (TValue).
        /// The target type is obtained via <see cref="TypeExtensions.GetArgumentsOfInheritedOpenGenericType"/> during initialization:
        /// given <c>SerializedDictionaryValueDrawer&lt;TValue&gt;</c> and its base class <c>EasyValueDrawer&lt;SerializedDictionary&lt;string, TValue&gt;&gt;</c>,
        /// the method extracts the generic argument <c>SerializedDictionary&lt;string, TValue&gt;</c>.
        /// </para>
        /// <para>
        /// When the actual value type is <c>SerializedDictionary&lt;string, int&gt;</c> (targets[0]), this method:
        /// </para>
        /// <list type="number">
        /// <item><description>Validates there's exactly one target type and that the element's target is a generic type.</description></item>
        /// <item><description>If the target is a completely concrete type (no generic parameters), requires exact type equality.</description></item>
        /// <item><description>Otherwise, calls <see cref="TypeExtensions.ResolveMissingGenericTypeArguments"/> to extract the missing generic arguments:
        /// <c>ResolveMissingGenericTypeArguments(SerializedDictionary&lt;string, TValue&gt;, SerializedDictionary&lt;string, int&gt;, false)</c>
        /// returns <c>[int]</c>, which is the concrete type for the parameter TValue.</description></item>
        /// <item><description>If no arguments are missing (types already match or are incompatible), returns null.</description></item>
        /// <item><description>Checks that <c>SerializedDictionaryValueDrawer&lt;TValue&gt;</c>'s generic constraints are satisfied by <c>[int]</c> via <see cref="TypeExtensions.AreGenericConstraintsSatisfiedBy"/>.</description></item>
        /// <item><description>If all checks pass, constructs and returns <c>SerializedDictionaryValueDrawer&lt;int&gt;</c> using <see cref="Type.MakeGenericType"/>.</description></item>
        /// </list>
        /// <para>
        /// This mechanism allows inspector elements to be automatically instantiated with the correct type arguments
        /// based on the actual runtime type of the property being inspected. It supports complex scenarios with multiple
        /// generic parameters, such as <c>KeyValuePairDrawer&lt;TKey, TValue&gt; : EasyValueDrawer&lt;KeyValuePair&lt;int, TKey&gt;&gt;</c>
        /// where only a subset of the target type's generic parameters correspond to the drawer's own generic parameters.
        /// </para>
        /// </remarks>
        private static Type GetMatchedType(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (targets.Length != 1) return null;
            if (!matchIndex.Targets[0].IsGenericTypeDefinition) return null;

            var valueType = targets[0];
            var argType = matchIndex.Targets[0];

            // If the argument is not a generic parameter and is a concrete type without generic parameters,
            // the element's target type must exactly match the value type.
            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                if (argType == valueType)
                {
                    return matchIndex.Type;
                }

                return null;
            }

            var missingArgs = argType.ResolveMissingGenericTypeArguments(valueType, false);
            if (missingArgs.Length == 0)
                return null;

            if (matchIndex.Type.AreGenericConstraintsSatisfiedBy(missingArgs))
            {
                var concreteType = matchIndex.Type.MakeGenericType(missingArgs);
                return concreteType;
            }

            return null;
        }

        /// <summary>
        /// Gets the priority of an inspector element type.
        /// Priority is obtained from the <see cref="IPriorityAccessor"/> attribute if present;
        /// otherwise, falls back to <see cref="NullPriorityFallbacks"/> if set.
        /// </summary>
        /// <param name="elementType">The element type to examine.</param>
        /// <returns>The priority of the element, or null if no priority could be determined.</returns>
        private static Priority GetElementPriority(Type elementType)
        {
            Priority priority = null;

            if (elementType.GetCustomAttributes(true)
                    .FirstOrDefault(attr => attr is IPriorityAccessor) is IPriorityAccessor priorityAttribute)
            {
                priority = priorityAttribute.Priority;
            }

            if (priority == null && NullPriorityFallbacks.Count > 0)
            {
                foreach (var fallback in NullPriorityFallbacks)
                {
                    priority = fallback(elementType);
                    if (priority != null)
                    {
                        break;
                    }
                }
            }

            return priority ?? Priority.Default;
        }

        /// <summary>
        /// Determines whether the specified handler type can handle the given element.
        /// Creates an instance of the handler type and calls its <see cref="IHandler.CanHandle(IElement)"/> method.
        /// </summary>
        /// <param name="handlerType">The handler type to test.</param>
        /// <param name="element">The inspector element to check.</param>
        /// <returns>True if the handler can handle the element; otherwise, false.</returns>
        private static bool CanHandleElement(Type handlerType, IElement element)
        {
            var handler = (IHandler)FormatterServices.GetUninitializedObject(handlerType);
            return handler.CanHandle(element);
        }
    }
}

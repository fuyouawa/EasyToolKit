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
        private static ITypeMatcher s_typeMatcher;
        private static bool s_typeMatcherInitialized;
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Gets or sets a callback that provides a default <see cref="Priority"/> when no priority attribute is found.
        /// If not set, returns null when no priority attribute is present.
        /// </summary>
        private static readonly List<Func<Type, Priority>> NullPriorityFallbacks = new List<Func<Type, Priority>>();

        public static ITypeMatcher TypeMatcher
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

            s_typeMatcher = TypeMatcherFactory.CreateDefault();
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
        }

        public static Type GetFirstElementType(IElement element, Func<Type, bool> typeFilter = null, IList<Type[]> additionalMatchTypesList = null)
        {
            var results = GetHandlerTypeResults(element, additionalMatchTypesList);
            foreach (var result in results)
            {
                var type = result.MatchedType;
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

                return type;
            }

            return null;
        }

        public static IEnumerable<Type> GetHandlerTypes(IElement element, Func<Type, bool> typeFilter = null, IList<Type[]> additionalMatchTypesList = null)
        {
            var results = GetHandlerTypeResults(element, additionalMatchTypesList);

            var typeSet = new HashSet<Type>();
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

        private static TypeMatchResult[] GetHandlerTypeResults(IElement element, IList<Type[]> additionalMatchTypesList = null)
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
            return TypeMatcherResults.GetMergedResults(resultsList);
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

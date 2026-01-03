using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that resolves generic type arguments for partially open generic types.
    /// </summary>
    /// <remarks>
    /// This rule handles complex scenarios where a handler/serializer's generic parameters correspond to
    /// specific type parameters within a partially concrete generic target type. It enables automatic
    /// instantiation of handlers with the correct type arguments based on the actual runtime type.
    /// </remarks>
    public static class GenericTypeResolutionRule
    {
        /// <summary>
        /// Gets the generic type resolution rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs generic type resolution.</value>
        public static readonly TypeMatchRule Rule = GenericTypeResolution;

        /// <summary>
        /// Resolves generic type arguments for handlers that target partially open generic types.
        /// Handles complex scenarios where a handler's generic parameters correspond to specific type
        /// parameters within a partially concrete generic target type.
        /// </summary>
        /// <param name="matchIndex">The type match index containing the handler type and its target generic type.</param>
        /// <param name="targets">The target types to match against; expected to contain exactly one type (the actual value type).</param>
        /// <param name="stopMatch">Reference parameter that can be set to true to stop further matching (not used in this implementation).</param>
        /// <returns>
        /// If the handler's target type is a (partially open) generic type and the actual value type
        /// can provide the missing generic arguments, returns the concretely instantiated handler type.
        /// Otherwise, returns null to indicate no match.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This rule enables handlers to target complex generic types with mixed concrete and generic type parameters.
        /// For example, consider a handler type that draws values of a generic collection with a fixed key type:
        /// </para>
        /// <code>
        /// class SerializedDictionaryValueDrawer&lt;TValue&gt; : EasyValueDrawer&lt;SerializedDictionary&lt;string, TValue&gt;&gt;
        /// </code>
        /// <para>
        /// Here, <c>SerializedDictionaryValueDrawer&lt;TValue&gt;</c> (matchIndex.Type) targets
        /// <c>SerializedDictionary&lt;string, TValue&gt;</c> (matchIndex.Targets[0]),
        /// which is a partially open generic type with a concrete key type (string) and a generic value type (TValue).
        /// The target type is obtained via <see cref="TypeExtensions.GetArgumentsOfInheritedOpenGenericType"/> during initialization.
        /// </para>
        /// <para>
        /// When the actual value type is <c>SerializedDictionary&lt;string, int&gt;</c> (targets[0]), this rule:
        /// </para>
        /// <list type="number">
        /// <item><description>Validates there's exactly one target type and that the handler's target is a generic type definition.</description></item>
        /// <item><description>If the target is a completely concrete type (no generic parameters), requires exact type equality.</description></item>
        /// <item><description>Otherwise, calls <see cref="TypeExtensions.ExtractGenericArgumentsFrom"/> to extract the missing generic arguments:
        /// <c>ExtractGenericArgumentsFrom(SerializedDictionary&lt;string, TValue&gt;, SerializedDictionary&lt;string, int&gt;, false)</c>
        /// returns <c>[int]</c>, which is the concrete type for the parameter TValue.</description></item>
        /// <item><description>If no arguments are missing (types already match or are incompatible), returns null.</description></item>
        /// <item><description>Checks that <c>SerializedDictionaryValueDrawer&lt;TValue&gt;</c>'s generic constraints are satisfied by <c>[int]</c> via <see cref="TypeExtensions.AreGenericConstraintsSatisfiedBy"/>.</description></item>
        /// <item><description>If all checks pass, constructs and returns <c>SerializedDictionaryValueDrawer&lt;int&gt;</c> using <see cref="Type.MakeGenericType"/>.</description></item>
        /// </list>
        /// <para>
        /// This mechanism allows handlers to be automatically instantiated with the correct type arguments
        /// based on the actual runtime type of the property being inspected. It supports complex scenarios with multiple
        /// generic parameters, such as <c>KeyValuePairDrawer&lt;TKey, TValue&gt; : EasyValueDrawer&lt;KeyValuePair&lt;int, TKey&gt;&gt;</c>
        /// where only a subset of the target type's generic parameters correspond to the handler's own generic parameters.
        /// </para>
        /// </remarks>
        public static Type GenericTypeResolution(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (targets.Length != 1) return null;
            if (!matchIndex.Targets[0].IsGenericTypeDefinition) return null;

            var valueType = targets[0];
            var argType = matchIndex.Targets[0];

            // If the argument is not a generic parameter and is a concrete type without generic parameters,
            // the handler's target type must exactly match the value type.
            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                if (argType == valueType)
                {
                    return matchIndex.Type;
                }

                return null;
            }

            var missingArgs = argType.ExtractGenericArgumentsFrom(valueType, true);
            if (missingArgs.Length == 0)
                return null;

            if (matchIndex.Type.AreGenericConstraintsSatisfiedBy(missingArgs))
            {
                var concreteType = matchIndex.Type.MakeGenericType(missingArgs);
                return concreteType;
            }

            return null;
        }
    }
}

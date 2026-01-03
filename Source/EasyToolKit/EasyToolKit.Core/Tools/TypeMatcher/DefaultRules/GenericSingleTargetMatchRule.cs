using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that matches generic type definitions against single generic target types.
    /// </summary>
    /// <remarks>
    /// This rule handles cases where a generic type definition should match against a single
    /// target type that has the same generic type definition. This is useful for matching
    /// handlers or serializers that operate on generic types with the same structure.
    /// </remarks>
    public static class GenericSingleTargetMatchRule
    {
        /// <summary>
        /// Gets the generic single target match rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs generic single target matching.</value>
        public static readonly TypeMatchRule Rule = GenericSingleTargetMatch;

        /// <summary>
        /// Matches generic type definitions against single generic target types.
        /// The target type must have the same generic type definition as the match index target.
        /// </summary>
        /// <param name="matchIndex">The type match index to evaluate.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <param name="stopMatch">When set to true, stops further rule evaluation for this match index.</param>
        /// <returns>The constructed generic type if successful; otherwise, null.</returns>
        /// <remarks>
        /// This rule requires that both the match index and the target are generic types
        /// with the same generic type definition. The rule then constructs a new generic
        /// type using the target's generic arguments.
        /// </remarks>
        public static Type GenericSingleTargetMatch(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (!matchIndex.Type.IsGenericTypeDefinition) return null;
            if (targets.Length != 1) return null;
            if (!matchIndex.Targets[0].IsGenericType || !targets[0].IsGenericType) return null;
            if (matchIndex.Targets[0].GetGenericTypeDefinition() != targets[0].GetGenericTypeDefinition()) return null;

            var matchArgs = matchIndex.Type.GetGenericArguments();
            var matchTargetArgs = matchIndex.Targets[0].GetGenericArguments();
            var targetArgs = targets[0].GetGenericArguments();

            if (matchArgs.Length != matchTargetArgs.Length || matchArgs.Length != targetArgs.Length) return null;

            if (!matchIndex.Type.AreGenericConstraintsSatisfiedBy(targetArgs)) return null;

            return matchIndex.Type.MakeGenericType(targetArgs);
        }
    }
}

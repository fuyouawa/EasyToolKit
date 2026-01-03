using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that matches types exactly.
    /// </summary>
    /// <remarks>
    /// This rule performs direct type equality checking. Both the type and all
    /// target types must match exactly for the rule to succeed. This is the
    /// most specific matching rule and is evaluated first.
    /// </remarks>
    public static class ExactMatchRule
    {
        /// <summary>
        /// Gets the exact match rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs exact type matching.</value>
        public static readonly TypeMatchRule Rule = ExactMatch;

        /// <summary>
        /// Matches types exactly - both the type and all target types must match exactly.
        /// </summary>
        /// <param name="matchIndex">The type match index to evaluate.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <param name="stopMatch">When set to true, stops further rule evaluation for this match index.</param>
        /// <returns>The matched type if successful; otherwise, null.</returns>
        /// <remarks>
        /// This rule only matches non-generic types. Generic type definitions
        /// are not matched by this rule.
        /// </remarks>
        public static Type ExactMatch(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (matchIndex.Type.IsGenericTypeDefinition) return null;
            if (targets.Length != matchIndex.Targets.Length) return null;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != matchIndex.Targets[i]) return null;
            }

            return matchIndex.Type;
        }
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that validates generic parameter constraints.
    /// </summary>
    /// <remarks>
    /// This rule handles cases where all target types are generic parameters and
    /// must satisfy the generic constraints of the match index type. This is
    /// useful for validating that generic type arguments meet the required
    /// constraints (class, struct, new(), base class, interface, etc.).
    /// </remarks>
    public static class GenericConstraintsMatchRule
    {
        /// <summary>
        /// Gets the generic constraints match rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs generic constraints matching.</value>
        public static readonly TypeMatchRule Rule = TargetsSatisfyGenericParameterConstraints;

        /// <summary>
        /// Matches when all target types are generic parameters and satisfy the generic constraints
        /// of the match index type.
        /// </summary>
        /// <param name="matchIndex">The type match index to evaluate.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <param name="stopMatch">When set to true, stops further rule evaluation for this match index.</param>
        /// <returns>The constructed generic type if successful; otherwise, null.</returns>
        /// <remarks>
        /// This rule verifies that all target types in the match index are generic parameters
        /// (e.g., <typeparamref name="T"/>, <typeparamref name="TKey"/>) and that the actual
        /// target types satisfy all generic constraints defined on the match index type.
        /// </remarks>
        public static Type TargetsSatisfyGenericParameterConstraints(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            for (int i = 0; i < matchIndex.Targets.Length; i++)
            {
                if (!matchIndex.Targets[i].IsGenericParameter) return null;
            }

            if (matchIndex.Type.IsGenericType && matchIndex.Type.AreGenericConstraintsSatisfiedBy(targets))
            {
                return matchIndex.Type.MakeGenericType(targets);
            }

            return null;
        }
    }
}

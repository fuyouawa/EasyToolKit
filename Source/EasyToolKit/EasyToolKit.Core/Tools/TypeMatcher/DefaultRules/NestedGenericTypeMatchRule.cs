using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that matches nested types within the same generic type definition.
    /// </summary>
    /// <remarks>
    /// This rule handles cases where nested types are declared within generic types.
    /// It matches when the declaring types of both the match index and the target have
    /// the same generic type definition. This is useful for matching handlers or serializers
    /// for nested types like <c>MyGenericClass&lt;T&gt;.NestedClass</c>.
    /// </remarks>
    public static class NestedGenericTypeMatchRule
    {
        /// <summary>
        /// Gets the nested generic type match rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs nested generic type matching.</value>
        public static readonly TypeMatchRule Rule = NestedInSameGenericType;

        /// <summary>
        /// Matches nested types that are declared within the same generic type definition.
        /// The declaring types must have the same generic type definition.
        /// </summary>
        /// <param name="matchIndex">The type match index to evaluate.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <param name="stopMatch">When set to true, stops further rule evaluation for this match index.</param>
        /// <returns>The constructed generic type if successful; otherwise, null.</returns>
        /// <remarks>
        /// This rule requires that:
        /// <list type="bullet">
        /// <item><description>All types (match index, its target, and the actual target) are nested</description></item>
        /// <item><description>All declaring types are generic types</description></item>
        /// <item><description>All declaring types have the same generic type definition</description></item>
        /// <item><description>The generic constraints are satisfied by the target's generic arguments</description></item>
        /// </list>
        /// </remarks>
        public static Type NestedInSameGenericType(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            if (targets.Length != 1) return null;

            var target = targets[0];

            if (!matchIndex.Type.IsNested || !matchIndex.Targets[0].IsNested || !target.IsNested) return null;

            if (!matchIndex.Type.DeclaringType.IsGenericType ||
                !matchIndex.Targets[0].DeclaringType.IsGenericType ||
                !target.DeclaringType.IsGenericType)
            {
                return null;
            }

            if (matchIndex.Type.DeclaringType.GetGenericTypeDefinition() != matchIndex.Targets[0].DeclaringType.GetGenericTypeDefinition() ||
                matchIndex.Type.DeclaringType.GetGenericTypeDefinition() != target.DeclaringType.GetGenericTypeDefinition()) return null;

            var args = target.GetGenericArguments();

            if (matchIndex.Type.AreGenericConstraintsSatisfiedBy(args))
            {
                return matchIndex.Type.MakeGenericType(args);
            }

            return null;
        }
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a method that determines if a type match index matches against target types.
    /// </summary>
    /// <param name="matchIndex">The type match index to evaluate.</param>
    /// <param name="targets">The target types to match against.</param>
    /// <param name="stopMatch">When set to true by the rule, stops further rule evaluation for this match index.</param>
    /// <returns>The matched type if successful; otherwise, null.</returns>
    /// <remarks>
    /// <para>
    /// Type match rules are used to implement custom matching logic. Each rule is called
    /// with a match index and target types, and should return the matched type if the rule
    /// successfully matches, or null if it does not.
    /// </para>
    /// <para>
    /// The <paramref name="stopMatch"/> parameter allows a rule to indicate that no further
    /// rules should be evaluated for this match index. This is useful when a rule has made
    /// a definitive determination and subsequent rules would be redundant or incorrect.
    /// </para>
    /// <para>
    /// For generic types, the returned type should be the fully constructed generic type,
    /// not the generic type definition. Use <see cref="Type.MakeGenericType(Type[])"/> to
    /// construct the closed generic type.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public static Type ExactMatch(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
    /// {
    ///     if (matchIndex.Type.IsGenericTypeDefinition) return null;
    ///     if (targets.Length != matchIndex.Targets.Length) return null;
    ///
    ///     for (int i = 0; i &lt; targets.Length; i++)
    ///     {
    ///         if (targets[i] != matchIndex.Targets[i]) return null;
    ///     }
    ///
    ///     return matchIndex.Type;
    /// }
    /// </code>
    /// </example>
    public delegate Type TypeMatchRule(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch);
}

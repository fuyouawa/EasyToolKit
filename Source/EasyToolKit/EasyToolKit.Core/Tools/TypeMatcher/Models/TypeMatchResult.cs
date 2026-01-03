using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents the result of a type matching operation.
    /// </summary>
    /// <remarks>
    /// Type match results contain detailed information about a successful match,
    /// including the matched type, the targets that were matched, the rule that
    /// produced the match, and the index that was used.
    /// </remarks>
    public class TypeMatchResult
    {
        /// <summary>
        /// Gets the match index that was used for this match.
        /// </summary>
        /// <value>
        /// The <see cref="TypeMatchIndex"/> that defines the matched type and its priority.
        /// </value>
        public TypeMatchIndex MatchIndex { get; }

        /// <summary>
        /// Gets the actual type that was matched.
        /// </summary>
        /// <value>
        /// The concrete type that was constructed or matched. For generic types,
        /// this is the closed generic type, not the generic type definition.
        /// </value>
        public Type MatchedType { get; }

        /// <summary>
        /// Gets the target types that were matched against.
        /// </summary>
        /// <value>
        /// An array of target types that were used in the matching operation.
        /// </value>
        public Type[] MatchTargets { get; }

        /// <summary>
        /// Gets the match rule that produced this result.
        /// </summary>
        /// <value>
        /// The <see cref="TypeMatchRule"/> delegate that successfully matched
        /// the index against the targets.
        /// </value>
        public TypeMatchRule MatchRule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatchResult"/> class.
        /// </summary>
        /// <param name="matchIndex">The match index that was used.</param>
        /// <param name="matchedType">The actual type that was matched.</param>
        /// <param name="matchTargets">The target types that were matched against.</param>
        /// <param name="matchRule">The match rule that produced this result.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any parameter is null.
        /// </exception>
        public TypeMatchResult(TypeMatchIndex matchIndex, Type matchedType, Type[] matchTargets,
            TypeMatchRule matchRule)
        {
            MatchIndex = matchIndex ?? throw new ArgumentNullException(nameof(matchIndex));
            MatchedType = matchedType ?? throw new ArgumentNullException(nameof(matchedType));
            MatchTargets = matchTargets ?? Type.EmptyTypes;
            MatchRule = matchRule ?? throw new ArgumentNullException(nameof(matchRule));
        }
    }
}

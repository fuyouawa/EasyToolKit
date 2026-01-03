using System;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for type matcher operations.
    /// </summary>
    /// <remarks>
    /// This static class provides convenient extension methods for working with
    /// type match results, making common operations more concise and readable.
    /// </remarks>
    public static class TypeMatcherExtensions
    {
        /// <summary>
        /// Gets the first matching type from the results.
        /// </summary>
        /// <param name="results">The match results.</param>
        /// <returns>The first matched type, or null if no matches.</returns>
        /// <remarks>
        /// This is a convenience method for getting the highest priority match
        /// from a sorted array of match results.
        /// </remarks>
        /// <example>
        /// <code>
        /// var results = matcher.GetMatches(typeof(int));
        /// Type bestMatch = results.GetFirstMatch();
        /// </code>
        /// </example>
        public static Type GetFirstMatch(this TypeMatchResult[] results)
        {
            return results.Length > 0 ? results[0].MatchedType : null;
        }

        /// <summary>
        /// Filters match results by type predicate.
        /// </summary>
        /// <param name="results">The match results.</param>
        /// <param name="predicate">The predicate to filter by.</param>
        /// <returns>Filtered match results.</returns>
        /// <remarks>
        /// This method filters the match results based on a predicate that
        /// operates on the matched type.
        /// </remarks>
        /// <example>
        /// <code>
        /// var results = matcher.GetMatches(typeof(int));
        /// var genericResults = results.Where(t => t.IsGenericType);
        /// </code>
        /// </example>
        public static TypeMatchResult[] Where(
            this TypeMatchResult[] results,
            Func<Type, bool> predicate)
        {
            return results.Where(r => predicate(r.MatchedType)).ToArray();
        }
    }
}

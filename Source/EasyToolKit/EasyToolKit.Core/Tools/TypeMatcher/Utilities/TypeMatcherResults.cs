using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides utility methods for working with type match results.
    /// </summary>
    /// <remarks>
    /// This static class provides utility methods for merging and processing
    /// type match results, including cached operations for performance optimization.
    /// </remarks>
    public static class TypeMatcherResults
    {
        /// <summary>
        /// Gets merged results from multiple type match result arrays.
        /// Results are merged and cached based on the input arrays to improve performance.
        /// </summary>
        /// <param name="resultsList">The list of type match result arrays to merge.</param>
        /// <returns>A merged array of type match results, ordered by priority (highest first).</returns>
        /// <remarks>
        /// This method merges multiple result arrays into a single array, sorted by priority.
        /// The results are cached based on the hash of the input array list, so subsequent
        /// calls with the same inputs will return cached results.
        /// </remarks>
        public static TypeMatchResult[] GetMergedResults(IReadOnlyList<TypeMatchResult[]> resultsList)
        {
            if (resultsList.Count == 0)
            {
                return Array.Empty<TypeMatchResult>();
            }

            if (resultsList.Count == 1)
            {
                return resultsList[0];
            }

            var hash = new HashCode();
            foreach (var value in resultsList)
            {
                hash.Add(value);
            }
            var hashCode = hash.ToHashCode();

            return TypeMatcherCache.GetOrAddMergedResults(hashCode, () =>
                resultsList
                    .SelectMany(x => x)
                    .OrderByDescending(result => result.MatchIndex.Priority)
                    .ToArray()
            );
        }
    }
}

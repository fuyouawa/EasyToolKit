using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Manages caching for type matcher operations.
    /// </summary>
    /// <remarks>
    /// This internal class provides centralized cache management for type matcher operations,
    /// including the merged results cache shared across all type matcher instances.
    /// </remarks>
    internal static class TypeMatcherCache
    {
        private static readonly Dictionary<int, TypeMatchResult[]> MergedResultsCache =
            new Dictionary<int, TypeMatchResult[]>();

        /// <summary>
        /// Clears the merged results cache.
        /// </summary>
        /// <remarks>
        /// This method should be called when match indices or rules change,
        /// as cached results may no longer be valid.
        /// </remarks>
        public static void ClearMergedResultsCache()
        {
            MergedResultsCache.Clear();
        }

        /// <summary>
        /// Gets cached merged results or adds them using the specified factory function.
        /// </summary>
        /// <param name="hashCode">The hash code representing the cache key.</param>
        /// <param name="factory">The factory function to create the results if not cached.</param>
        /// <returns>The cached or newly created merged results.</returns>
        /// <remarks>
        /// This method provides a thread-safe way to get or add cached results.
        /// The hash code should uniquely identify the input data used to generate the results.
        /// </remarks>
        public static TypeMatchResult[] GetOrAddMergedResults(
            int hashCode,
            Func<TypeMatchResult[]> factory)
        {
            if (MergedResultsCache.TryGetValue(hashCode, out var results))
            {
                return results;
            }

            results = factory();
            MergedResultsCache[hashCode] = results;
            return results;
        }

        /// <summary>
        /// Tries to get cached merged results for the specified hash code.
        /// </summary>
        /// <param name="hashCode">The hash code representing the cache key.</param>
        /// <param name="results">The cached results, if found.</param>
        /// <returns><c>true</c> if cached results were found; otherwise, <c>false</c>.</returns>
        public static bool TryGetMergedResults(int hashCode, out TypeMatchResult[] results)
        {
            return MergedResultsCache.TryGetValue(hashCode, out results);
        }
    }
}

using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Abstract base class for type matchers providing common functionality.
    /// </summary>
    /// <remarks>
    /// This base class provides shared functionality for all type matcher implementations,
    /// including match rule management, cache management, and default rule registration.
    /// Derived classes must implement the abstract methods for managing match indices
    /// and getting match results.
    /// </remarks>
    public abstract class TypeMatcherBase : ITypeMatcher
    {
        /// <summary>
        /// The collection of registered match rules.
        /// </summary>
        protected readonly List<TypeMatchRule> MatchRules = new List<TypeMatchRule>();

        /// <summary>
        /// The collection of registered match indices.
        /// </summary>
        protected List<TypeMatchIndex> MatchIndices;

        /// <summary>
        /// The cache for match results, keyed by target type hash code.
        /// </summary>
        protected readonly Dictionary<int, TypeMatchResult[]> MatchResultsCache =
            new Dictionary<int, TypeMatchResult[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatcherBase"/> class.
        /// </summary>
        /// <param name="addDefaultRules">Whether to add the default type matching rules.</param>
        protected TypeMatcherBase(bool addDefaultRules)
        {
            if (addDefaultRules)
            {
                AddDefaultMatchRules();
            }
        }

        /// <summary>
        /// Adds type match indices to the current collection.
        /// </summary>
        /// <param name="matchIndices">The type match indices to add.</param>
        /// <remarks>
        /// This method must be implemented by derived classes to add indices
        /// to the collection and clear the cache.
        /// </remarks>
        public abstract void AddTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices);

        /// <summary>
        /// Replaces the current type match indices with the specified collection.
        /// </summary>
        /// <param name="matchIndices">The new type match indices to use.</param>
        /// <remarks>
        /// This method must be implemented by derived classes to replace indices
        /// in the collection and clear the cache.
        /// </remarks>
        public abstract void SetTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices);

        /// <summary>
        /// Adds a custom match rule to the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to add.</param>
        /// <remarks>
        /// This method must be implemented by derived classes to add the rule
        /// to the collection and clear the cache.
        /// </remarks>
        public abstract void AddMatchRule(TypeMatchRule rule);

        /// <summary>
        /// Removes a match rule from the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to remove.</param>
        /// <remarks>
        /// This method must be implemented by derived classes to remove the rule
        /// from the collection and clear the cache.
        /// </remarks>
        public abstract void RemoveMatchRule(TypeMatchRule rule);

        /// <summary>
        /// Gets type matches for the specified target types.
        /// Results are cached based on the target types to improve performance.
        /// </summary>
        /// <param name="targets">The target types to match against.</param>
        /// <returns>An array of type match results, ordered by priority (highest first).</returns>
        /// <remarks>
        /// This method must be implemented by derived classes to perform the
        /// actual matching logic.
        /// </remarks>
        public abstract TypeMatchResult[] GetMatches(params Type[] targets);

        /// <summary>
        /// Clears all caches.
        /// </summary>
        /// <remarks>
        /// This method clears both the instance match results cache and the
        /// static merged results cache. It should be called whenever match
        /// indices or rules are modified.
        /// </remarks>
        protected void ClearCache()
        {
            MatchResultsCache.Clear();
            TypeMatcherCache.ClearMergedResultsCache();
        }

        /// <summary>
        /// Adds the default type matching rules to the matcher.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="DefaultTypeMatchRuleProvider"/> to get all
        /// default rules and adds them to the matcher. Rules are added in the
        /// order returned by the provider, which is significant for rule evaluation.
        /// </remarks>
        protected virtual void AddDefaultMatchRules()
        {
            var provider = new DefaultTypeMatchRuleProvider();
            foreach (var rule in provider.GetRules())
            {
                AddMatchRule(rule);
            }
        }
    }
}

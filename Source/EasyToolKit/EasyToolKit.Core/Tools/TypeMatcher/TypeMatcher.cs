using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a type matching index that defines a type and its target types for matching.
    /// </summary>
    public class TypeMatchIndex
    {
        /// <summary>
        /// Gets or sets the type to be matched.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the priority of this match index (higher values have higher priority).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the target types that this index should match against.
        /// </summary>
        public Type[] Targets { get; set; }

        /// <summary>
        /// Initializes a new instance of the TypeMatchIndex class.
        /// </summary>
        /// <param name="type">The type to be matched.</param>
        /// <param name="priority">The priority of this match index.</param>
        /// <param name="targets">The target types to match against.</param>
        public TypeMatchIndex(Type type, int priority, Type[] targets)
        {
            Type = type;
            Priority = priority;
            Targets = targets ?? Type.EmptyTypes;
        }
    }

    /// <summary>
    /// Represents the result of a type matching operation.
    /// </summary>
    public class TypeMatchResult
    {
        /// <summary>
        /// Gets the match index that was used for this match.
        /// </summary>
        public TypeMatchIndex MatchIndex { get; }

        /// <summary>
        /// Gets the actual type that was matched.
        /// </summary>
        public Type MatchedType { get; }

        /// <summary>
        /// Gets the target types that were matched against.
        /// </summary>
        public Type[] MatchTargets { get; }

        /// <summary>
        /// Gets the match rule that produced this result.
        /// </summary>
        public TypeMatchRule MatchRule { get; }

        /// <summary>
        /// Initializes a new instance of the TypeMatchResult class.
        /// </summary>
        /// <param name="matchIndex">The match index that was used.</param>
        /// <param name="matchedType">The actual type that was matched.</param>
        /// <param name="matchTargets">The target types that were matched against.</param>
        /// <param name="matchRule">The match rule that produced this result.</param>
        public TypeMatchResult(TypeMatchIndex matchIndex, Type matchedType, Type[] matchTargets,
            TypeMatchRule matchRule)
        {
            MatchIndex = matchIndex;
            MatchedType = matchedType;
            MatchTargets = matchTargets;
            MatchRule = matchRule;
        }
    }

    /// <summary>
    /// Represents a method that determines if a type match index matches against target types.
    /// </summary>
    /// <param name="matchIndex">The type match index to evaluate.</param>
    /// <param name="targets">The target types to match against.</param>
    /// <param name="stopMatch">When set to true by the rule, stops further rule evaluation for this match index.</param>
    /// <returns>The matched type if successful; otherwise, null.</returns>
    public delegate Type TypeMatchRule(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch);

    /// <summary>
    /// Provides a flexible type matching system that can match types against target types using configurable rules.
    /// Supports exact matching, generic type matching, type inference, and custom matching rules.
    /// </summary>
    public class TypeMatcher
    {
        private static readonly Dictionary<int, TypeMatchResult[]> MergedResultsCache = new Dictionary<int, TypeMatchResult[]>();

        private List<TypeMatchIndex> _matchIndices;
        private readonly List<TypeMatchRule> _matchRules = new List<TypeMatchRule>();

        private readonly Dictionary<int, TypeMatchResult[]> _matchResultsCache = new Dictionary<int, TypeMatchResult[]>();

        /// <summary>
        /// Initializes a new instance of the TypeMatcher class.
        /// </summary>
        /// <param name="addDefaultMatchRules">Whether to add the default type matching rules.</param>
        public TypeMatcher(bool addDefaultMatchRules = true)
        {
            if (addDefaultMatchRules)
            {
                AddDefaultMatchRules();
            }
        }

        /// <summary>
        /// Adds type match indices to the current collection.
        /// </summary>
        /// <param name="matchIndices">The type match indices to add.</param>
        public void AddTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            _matchIndices.AddRange(matchIndices);
            ClearCache();
        }

        /// <summary>
        /// Replaces the current type match indices with the specified collection.
        /// </summary>
        /// <param name="matchIndices">The new type match indices to use.</param>
        public void SetTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            _matchIndices = matchIndices.ToList();
            ClearCache();
        }

        /// <summary>
        /// Adds a custom match rule to the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to add.</param>
        public void AddMatchRule(TypeMatchRule rule)
        {
            _matchRules.Add(rule);
            ClearCache();
        }

        /// <summary>
        /// Removes a match rule from the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to remove.</param>
        public void RemoveMatchRule(TypeMatchRule rule)
        {
            _matchRules.Remove(rule);
            ClearCache();
        }

        private void ClearCache()
        {
            _matchResultsCache.Clear();
            MergedResultsCache.Clear();
        }

        /// <summary>
        /// Gets type matches for the specified target types.
        /// Results are cached based on the target types to improve performance.
        /// </summary>
        /// <param name="targets">The target types to match against.</param>
        /// <returns>An array of type match results, ordered by priority (highest first).</returns>
        public TypeMatchResult[] GetMatches(params Type[] targets)
        {
            var hash = new HashCode();
            foreach (var target in targets)
            {
                hash.Add(target);
            }
            var hashCode = hash.ToHashCode();

            if (_matchResultsCache.TryGetValue(hashCode, out var ret))
            {
                return ret;
            }

            var results = new List<TypeMatchResult>();

            foreach (var index in _matchIndices)
            {
                if (index.Targets.Length != targets.Length)
                    continue;

                foreach (var rule in _matchRules)
                {
                    bool stop = false;
                    var match = rule(index, targets, ref stop);
                    if (match != null)
                        results.Add(new TypeMatchResult(index, match, targets, rule));

                    if (stop)
                        break;
                }
            }

            ret = results
                .OrderByDescending(result => result.MatchIndex.Priority)
                .ToArray();
            _matchResultsCache[hashCode] = ret;
            return ret;
        }

        /// <summary>
        /// Gets merged results from multiple type match result arrays.
        /// Results are merged and cached based on the input arrays to improve performance.
        /// </summary>
        /// <param name="resultsList">The list of type match result arrays to merge.</param>
        /// <returns>A merged array of type match results, ordered by priority (highest first).</returns>
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

            if (MergedResultsCache.TryGetValue(hashCode, out var results))
            {
                return results;
            }

            results = resultsList
                .SelectMany(x => x)
                .OrderByDescending(result => result.MatchIndex.Priority)
                .ToArray();
            MergedResultsCache[hashCode] = results;
            return results;
        }

        /// <summary>
        /// Adds the default type matching rules to the matcher.
        /// These include exact matching, generic type matching, type inference, and nested type matching.
        /// </summary>
        private void AddDefaultMatchRules()
        {
            _matchRules.Add(DefaultTypeMatchRules.ExactMatch);
            _matchRules.Add(DefaultTypeMatchRules.GenericSingleTargetMatch);
            _matchRules.Add(DefaultTypeMatchRules.TargetsSatisfyGenericParameterConstraints);
            _matchRules.Add(DefaultTypeMatchRules.GenericParameterInference);
            _matchRules.Add(DefaultTypeMatchRules.NestedInSameGenericType);
        }
    }
}

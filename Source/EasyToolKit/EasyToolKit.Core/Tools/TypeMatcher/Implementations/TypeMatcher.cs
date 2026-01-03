using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Provides a flexible type matching system that can match types against target types using configurable rules.
    /// Supports exact matching, generic type matching, type inference, and custom matching rules.
    /// </summary>
    /// <remarks>
    /// This is the default implementation of <see cref="ITypeMatcher"/>. It provides a comprehensive
    /// type matching system with caching for performance optimization. The type matcher is used
    /// primarily for finding appropriate handlers or serializers for given value types.
    /// </remarks>
    public sealed class TypeMatcher : TypeMatcherBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMatcher"/> class.
        /// </summary>
        /// <param name="addDefaultMatchRules">Whether to add the default type matching rules.</param>
        /// <remarks>
        /// When <paramref name="addDefaultMatchRules"/> is true, the following rules are added:
        /// <list type="bullet">
        /// <item><description>ExactMatch - Direct type equality</description></item>
        /// <item><description>GenericSingleTargetMatch - Generic type definition matching</description></item>
        /// <item><description>GenericConstraintsMatchRule - Generic constraint satisfaction</description></item>
        /// <item><description>GenericParameterInference - Generic parameter inference</description></item>
        /// <item><description>NestedGenericTypeMatch - Nested generic type matching</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("Use TypeMatcherFactory.CreateDefault() or TypeMatcherFactory.CreateEmpty() instead.")]
        public TypeMatcher(bool addDefaultMatchRules = true) : base(addDefaultMatchRules)
        {
            MatchIndices = new List<TypeMatchIndex>();
        }

        /// <summary>
        /// Adds type match indices to the current collection.
        /// </summary>
        /// <param name="matchIndices">The type match indices to add.</param>
        /// <remarks>
        /// This method appends the specified indices to the existing collection.
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        public override void AddTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            MatchIndices.AddRange(matchIndices);
            ClearCache();
        }

        /// <summary>
        /// Replaces the current type match indices with the specified collection.
        /// </summary>
        /// <param name="matchIndices">The new type match indices to use.</param>
        /// <remarks>
        /// This method replaces all existing indices with the specified collection.
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        public override void SetTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices)
        {
            MatchIndices = matchIndices.ToList();
            ClearCache();
        }

        /// <summary>
        /// Adds a custom match rule to the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to add.</param>
        /// <remarks>
        /// Rules are evaluated in the order they were added. The cache is automatically
        /// cleared after this operation.
        /// </remarks>
        public override void AddMatchRule(TypeMatchRule rule)
        {
            MatchRules.Add(rule);
            ClearCache();
        }

        /// <summary>
        /// Removes a match rule from the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to remove.</param>
        /// <remarks>
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        public override void RemoveMatchRule(TypeMatchRule rule)
        {
            MatchRules.Remove(rule);
            ClearCache();
        }

        /// <summary>
        /// Gets type matches for the specified target types.
        /// Results are cached based on the target types to improve performance.
        /// </summary>
        /// <param name="targets">The target types to match against.</param>
        /// <returns>An array of type match results, ordered by priority (highest first).</returns>
        /// <remarks>
        /// This method evaluates all registered match indices against the specified
        /// target types using the registered match rules. Results are cached for
        /// performance, so subsequent calls with the same target types will return
        /// cached results.
        /// </remarks>
        public override TypeMatchResult[] GetMatches(params Type[] targets)
        {
            var hash = new HashCode();
            foreach (var target in targets)
            {
                hash.Add(target);
            }
            var hashCode = hash.ToHashCode();

            if (MatchResultsCache.TryGetValue(hashCode, out var ret))
            {
                return ret;
            }

            var results = new List<TypeMatchResult>();

            foreach (var index in MatchIndices)
            {
                if (index.Targets.Length != targets.Length)
                    continue;

                foreach (var rule in MatchRules)
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
            MatchResultsCache[hashCode] = ret;
            return ret;
        }
    }
}

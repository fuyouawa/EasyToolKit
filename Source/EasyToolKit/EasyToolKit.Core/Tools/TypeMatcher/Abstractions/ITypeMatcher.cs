using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a type matcher that finds matching types based on configurable rules.
    /// </summary>
    /// <remarks>
    /// Type matchers are used to find appropriate handler or serializer types for
    /// given target types. They support exact matching, generic type matching, type
    /// inference, and custom matching rules through a flexible rule-based system.
    /// </remarks>
    public interface ITypeMatcher
    {
        /// <summary>
        /// Adds type match indices to the current collection.
        /// </summary>
        /// <param name="matchIndices">The type match indices to add.</param>
        /// <remarks>
        /// This method appends the specified indices to the existing collection.
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        void AddTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices);

        /// <summary>
        /// Replaces the current type match indices with the specified collection.
        /// </summary>
        /// <param name="matchIndices">The new type match indices to use.</param>
        /// <remarks>
        /// This method replaces all existing indices with the specified collection.
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        void SetTypeMatchIndices(IEnumerable<TypeMatchIndex> matchIndices);

        /// <summary>
        /// Adds a custom match rule to the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to add.</param>
        /// <remarks>
        /// Rules are evaluated in the order they were added. The cache is automatically
        /// cleared after this operation.
        /// </remarks>
        void AddMatchRule(TypeMatchRule rule);

        /// <summary>
        /// Removes a match rule from the type matcher.
        /// </summary>
        /// <param name="rule">The match rule to remove.</param>
        /// <remarks>
        /// The cache is automatically cleared after this operation.
        /// </remarks>
        void RemoveMatchRule(TypeMatchRule rule);

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
        TypeMatchResult[] GetMatches(params Type[] targets);
    }
}

using JetBrains.Annotations;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides factory methods for creating type matchers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This static class serves as the main entry point for creating type matcher instances.
    /// It provides simplified creation methods while hiding implementation details.
    /// </para>
    /// <para>
    /// <b>Usage Examples:</b>
    /// </para>
    /// <example>
    /// <code>
    /// // Example 1: Create with default rules
    /// var matcher1 = TypeMatcherFactory.CreateDefault();
    /// matcher1.SetTypeMatchIndices(indices);
    ///
    /// // Example 2: Create without rules (add custom rules)
    /// var matcher2 = TypeMatcherFactory.CreateEmpty();
    /// matcher2.AddMatchRule(MyCustomRule);
    ///
    /// // Example 3: Create with custom rules
    /// var matcher3 = TypeMatcherFactory.CreateWithRules(
    ///     MyRule1,
    ///     MyRule2
    /// );
    /// </code>
    /// </example>
    /// </remarks>
    public static class TypeMatcherFactory
    {
        /// <summary>
        /// Creates a type matcher with default match rules.
        /// </summary>
        /// <returns>A configured type matcher instance.</returns>
        /// <remarks>
        /// The default rules include:
        /// <list type="bullet">
        /// <item><description>ExactMatch - Direct type equality</description></item>
        /// <item><description>GenericSingleTargetMatch - Generic type definition matching</description></item>
        /// <item><description>GenericConstraintsMatchRule - Generic constraint satisfaction</description></item>
        /// <item><description>GenericParameterInference - Generic parameter inference</description></item>
        /// <item><description>NestedGenericTypeMatch - Nested generic type matching</description></item>
        /// </list>
        /// </remarks>
        [PublicAPI]
        public static ITypeMatcher CreateDefault()
        {
            return new Implementations.TypeMatcher(addDefaultMatchRules: true);
        }

        /// <summary>
        /// Creates a type matcher without default match rules.
        /// </summary>
        /// <returns>An empty type matcher instance.</returns>
        /// <remarks>
        /// Use this method when you want to add custom rules without the default rules.
        /// </remarks>
        [PublicAPI]
        public static ITypeMatcher CreateEmpty()
        {
            return new Implementations.TypeMatcher(addDefaultMatchRules: false);
        }

        /// <summary>
        /// Creates a type matcher with custom match rules.
        /// </summary>
        /// <param name="rules">The custom rules to add.</param>
        /// <returns>A configured type matcher instance.</returns>
        /// <remarks>
        /// This method creates an empty matcher and adds the specified rules to it.
        /// The default rules are not added.
        /// </remarks>
        [PublicAPI]
        public static ITypeMatcher CreateWithRules(params TypeMatchRule[] rules)
        {
            var matcher = new Implementations.TypeMatcher(addDefaultMatchRules: false);
            foreach (var rule in rules)
            {
                matcher.AddMatchRule(rule);
            }
            return matcher;
        }
    }
}

using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides the default type matching rules.
    /// </summary>
    /// <remarks>
    /// This class provides all default type matching rules in the correct order.
    /// Rules are evaluated in the order returned by <see cref="GetRules"/>, so the
    /// order is significant. More specific rules should come before more general ones.
    /// </remarks>
    public sealed class DefaultTypeMatchRuleProvider
    {
        /// <summary>
        /// Gets all default type match rules in priority order.
        /// </summary>
        /// <returns>An enumerable of default match rules.</returns>
        /// <remarks>
        /// The rules are returned in the order they should be evaluated:
        /// <list type="number">
        /// <item><description>ExactMatch - Direct type equality</description></item>
        /// <item><description>GenericSingleTargetMatch - Generic type definition matching</description></item>
        /// <item><description>GenericConstraintsMatchRule - Generic constraint satisfaction</description></item>
        /// <item><description>GenericParameterInference - Generic parameter inference</description></item>
        /// <item><description>GenericTypeResolution - Generic type argument resolution for partially open generic types</description></item>
        /// <item><description>NestedGenericTypeMatch - Nested generic type matching</description></item>
        /// </list>
        /// </remarks>
        public IEnumerable<TypeMatchRule> GetRules()
        {
            yield return ExactMatchRule.Rule;
            yield return GenericSingleTargetMatchRule.Rule;
            yield return GenericConstraintsMatchRule.Rule;
            yield return GenericParameterInferenceRule.Rule;
            yield return GenericTypeResolutionRule.Rule;
            yield return NestedGenericTypeMatchRule.Rule;
        }
    }
}

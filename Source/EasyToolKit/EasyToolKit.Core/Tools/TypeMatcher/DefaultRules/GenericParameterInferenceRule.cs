using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a type matching rule that performs generic parameter inference.
    /// </summary>
    /// <remarks>
    /// This rule handles complex generic type matching by inferring generic parameters
    /// from the target types. It supports scenarios where some target types are generic
    /// parameters and others are concrete types, allowing for flexible generic type resolution.
    /// </remarks>
    public static class GenericParameterInferenceRule
    {
        /// <summary>
        /// Gets the generic parameter inference rule delegate.
        /// </summary>
        /// <value>A <see cref="TypeMatchRule"/> delegate that performs generic parameter inference.</value>
        public static readonly TypeMatchRule Rule = GenericParameterInference;

        /// <summary>
        /// Performs generic parameter inference to match types.
        /// Infers generic parameters from the target types and constructs the appropriate generic type.
        /// </summary>
        /// <param name="matchIndex">The type match index to evaluate.</param>
        /// <param name="targets">The target types to match against.</param>
        /// <param name="stopMatch">When set to true, stops further rule evaluation for this match index.</param>
        /// <returns>The inferred generic type if successful; otherwise, null.</returns>
        /// <remarks>
        /// This rule supports two inference modes:
        /// <list type="bullet">
        /// <item><description>Full inference: All targets are generic parameters (e.g., <c>List&lt;T&gt;</c> targets <c>T[]</c>)</description></item>
        /// <item><description>Partial inference: Some targets are generic parameters, others are concrete (e.g., <c>Dictionary&lt;string, TValue&gt;</c> targets <c>string, TValue[]</c>)</description></item>
        /// </list>
        /// </remarks>
        public static Type GenericParameterInference(TypeMatchIndex matchIndex, Type[] targets,
            ref bool stopMatch)
        {
            Type[] inferTargets;

            // Make sure we can apply generic parameter inference to the match info
            {
                if (!matchIndex.Type.IsGenericType) return null;

                int genericParameterTargetCount = 0;

                for (int i = 0; i < matchIndex.Targets.Length; i++)
                {
                    if (matchIndex.Targets[i].IsGenericParameter)
                    {
                        genericParameterTargetCount++;
                    }
                    else if (matchIndex.Targets[i] != targets[i])
                    {
                        // Everything but generic parameters must match exactly
                        return null;
                    }
                }

                if (genericParameterTargetCount == 0) return null;

                if (genericParameterTargetCount != targets.Length)
                {
                    inferTargets = new Type[genericParameterTargetCount];
                    int count = 0;
                    for (int i = 0; i < matchIndex.Targets.Length; i++)
                    {
                        if (matchIndex.Targets[i].IsGenericParameter)
                        {
                            inferTargets[count++] = targets[i];
                        }
                    }
                }
                else
                {
                    inferTargets = targets;
                }
            }

            Type[] inferredArgs;

            if (matchIndex.Type.TryInferGenericParameters(out inferredArgs, inferTargets))
            {
                return matchIndex.Type.GetGenericTypeDefinition().MakeGenericType(inferredArgs);
            }

            return null;
        }
    }
}

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a builder for configuring expression evaluators.
    /// </summary>
    /// <typeparam name="TResult">The type of value to evaluate.</typeparam>
    /// <remarks>
    /// This builder provides a fluent API for configuring expression evaluators
    /// before building them. Use the <see cref="Build"/> method to create the
    /// final evaluator instance.
    /// </remarks>
    public interface IExpressionEvaluatorBuilder<out TResult>
    {
        /// <summary>
        /// Configures the evaluator to require an expression flag.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// <para>
        /// When enabled, the expression must start with '@' to trigger evaluation.
        /// Otherwise, it's treated as a primitive string value.
        /// </para>
        /// <para>
        /// This is useful for conditional interpolation where the same attribute
        /// can accept both static strings and dynamic expressions.
        /// </para>
        /// <example>
        /// <code>
        /// // With expression flag enabled
        /// var evaluator = ExpressionEvaluatorFactory
        ///     .Evaluate&lt;string&gt;(labelText, targetType)
        ///     .WithExpressionFlag();
        ///
        /// // Usage in attribute:
        /// // [Label("Static Label")]        // Returns "Static Label"
        /// // [Label("@DynamicLabel")]       // Evaluates 'DynamicLabel' property
        /// // [Label("@Player.Name.Length")] // Evaluates nested expression
        /// </code>
        /// </example>
        /// </remarks>
        IExpressionEvaluatorBuilder<TResult> WithExpressionFlag();

        /// <summary>
        /// Builds the configured expression evaluator.
        /// </summary>
        /// <returns>The configured expression evaluator instance.</returns>
        /// <remarks>
        /// After calling this method, the evaluator is ready for use.
        /// The builder should not be used after calling <see cref="Build"/>.
        /// </remarks>
        IExpressionEvaluator<TResult> Build();
    }
}

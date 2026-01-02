using JetBrains.Annotations;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Evaluates literal (non-interpolated) string values.
    /// </summary>
    /// <remarks>
    /// This evaluator returns the string value as-is without any expression parsing
    /// or evaluation. It's used for static text that should not be interpreted as
    /// an expression path.
    /// </remarks>
    public sealed class LiteralExpressionEvaluator : ExpressionEvaluatorBase<string>
    {
        private readonly string _literalValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpressionEvaluator"/> class.
        /// </summary>
        /// <param name="value">The literal string value to return.</param>
        /// <remarks>
        /// The value is returned as-is without modification or evaluation.
        /// </remarks>
        public LiteralExpressionEvaluator([CanBeNull] string value)
            : base(value ?? string.Empty)
        {
            _literalValue = value ?? string.Empty;
        }

        /// <summary>
        /// Evaluates the literal value (returns the stored string).
        /// </summary>
        /// <param name="context">The context object (ignored for literal evaluators).</param>
        /// <returns>The literal string value.</returns>
        /// <remarks>
        /// This method ignores the context parameter and always returns the
        /// literal value provided during construction.
        /// </remarks>
        protected override string EvaluateCore(object context)
        {
            return _literalValue;
        }

        /// <summary>
        /// Performs validation (literal expressions are always valid).
        /// </summary>
        /// <remarks>
        /// Literal expressions have no validation requirements - they are always valid.
        /// </remarks>
        protected override void PerformValidation()
        {
            SetError(null);
            base.PerformValidation();
        }

        /// <summary>
        /// Gets whether this evaluator has a validation error.
        /// </summary>
        /// <param name="errorMessage">Always set to <c>null</c>.</param>
        /// <returns>Always <c>false</c> (literal expressions are always valid).</returns>
        public override bool TryGetError(out string errorMessage)
        {
            errorMessage = null;
            return false;
        }
    }

    /// <summary>
    /// Typed literal expression evaluator for string values.
    /// </summary>
    /// <remarks>
    /// This is a strongly-typed version of <see cref="LiteralExpressionEvaluator"/>
    /// that implements <see cref="IExpressionEvaluator{TResult}"/>.
    /// </remarks>
    public sealed class LiteralExpressionEvaluator<TResult> : ExpressionEvaluatorBase<TResult>
    {
        private readonly TResult _literalValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralExpressionEvaluator{TResult}"/> class.
        /// </summary>
        /// <param name="value">The literal value to return.</param>
        /// <remarks>
        /// The value is returned as-is without modification or evaluation.
        /// For string types, null values are converted to string.Empty.
        /// </remarks>
        public LiteralExpressionEvaluator([CanBeNull] TResult value)
            : base(value?.ToString() ?? string.Empty)
        {
            // For string types, convert null to empty string
            if (typeof(TResult) == typeof(string) && value == null)
            {
                _literalValue = default;
            }
            else
            {
                _literalValue = value;
            }
        }

        /// <summary>
        /// Evaluates the literal value (returns the stored value).
        /// </summary>
        /// <param name="context">The context object (ignored for literal evaluators).</param>
        /// <returns>The literal value.</returns>
        /// <remarks>
        /// This method ignores the context parameter and always returns the
        /// literal value provided during construction.
        /// </remarks>
        protected override TResult EvaluateCore(object context)
        {
            return _literalValue;
        }

        /// <summary>
        /// Performs validation (literal expressions are always valid).
        /// </summary>
        /// <remarks>
        /// Literal expressions have no validation requirements - they are always valid.
        /// </remarks>
        protected override void PerformValidation()
        {
            SetError(null);
            base.PerformValidation();
        }

        /// <summary>
        /// Gets whether this evaluator has a validation error.
        /// </summary>
        /// <param name="errorMessage">Always set to <c>null</c>.</param>
        /// <returns>Always <c>false</c> (literal expressions are always valid).</returns>
        public override bool TryGetError(out string errorMessage)
        {
            errorMessage = null;
            return false;
        }
    }
}

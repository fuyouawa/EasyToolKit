using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides a base implementation for expression evaluators.
    /// </summary>
    /// <remarks>
    /// This abstract class implements common functionality for all expression evaluators,
    /// including error validation and deferred evaluation pattern. Subclasses must
    /// implement the actual evaluation logic.
    /// </remarks>
    public abstract class ExpressionEvaluatorBase : IExpressionEvaluator
    {
        /// <summary>
        /// The expression path to evaluate.
        /// </summary>
        protected readonly string ExpressionPath;

        /// <summary>
        /// The expected result type of the evaluation.
        /// </summary>
        protected readonly Type ExpectedType;

        private string _errorMessage;
        private bool _isValidated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluatorBase"/> class.
        /// </summary>
        /// <param name="expressionPath">The expression path to evaluate.</param>
        /// <param name="expectedType">The expected result type.</param>
        protected ExpressionEvaluatorBase(string expressionPath, Type expectedType)
        {
            ExpressionPath = expressionPath;
            ExpectedType = expectedType;
        }

        /// <summary>
        /// Gets whether this evaluator has a validation error.
        /// </summary>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>
        /// <c>true</c> if there is a validation error; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method uses deferred validation - the actual validation is performed
        /// on the first call. Subsequent calls return the cached result.
        /// </remarks>
        public virtual bool TryGetError(out string errorMessage)
        {
            if (!_isValidated)
            {
                PerformValidation();
                _isValidated = true;
            }

            errorMessage = _errorMessage;
            return _errorMessage != null;
        }

        /// <summary>
        /// Evaluates the expression against the specified context object.
        /// </summary>
        /// <param name="context">The context object to evaluate against.</param>
        /// <returns>The evaluated value as an untyped object.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the evaluator has a validation error.
        /// </exception>
        public abstract object Evaluate(object context);

        /// <summary>
        /// Performs validation logic for the expression.
        /// </summary>
        /// <remarks>
        /// Subclasses can override this method to provide custom validation logic.
        /// The default implementation performs no validation.
        /// </remarks>
        protected virtual void PerformValidation()
        {
            // Default implementation: no validation
            _errorMessage = null;
        }

        /// <summary>
        /// Sets the validation error message.
        /// </summary>
        /// <param name="errorMessage">The error message to set.</param>
        protected void SetError(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        /// <summary>
        /// Checks whether the evaluator has been validated.
        /// </summary>
        /// <returns>
        /// <c>true</c> if validation has been performed; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsValidated() => _isValidated;
    }

    /// <summary>
    /// Provides a base implementation for typed expression evaluators.
    /// </summary>
    /// <typeparam name="TResult">The type of value to evaluate.</typeparam>
    /// <remarks>
    /// This abstract class extends <see cref="ExpressionEvaluatorBase"/> with
    /// type-safe evaluation. Subclasses must implement the typed evaluation logic.
    /// </remarks>
    public abstract class ExpressionEvaluatorBase<TResult> : ExpressionEvaluatorBase, IExpressionEvaluator<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluatorBase{TResult}"/> class.
        /// </summary>
        /// <param name="expressionPath">The expression path to evaluate.</param>
        protected ExpressionEvaluatorBase(string expressionPath)
            : base(expressionPath, typeof(TResult))
        {
        }

        /// <summary>
        /// Evaluates the expression as an untyped object.
        /// </summary>
        /// <param name="context">The context object to evaluate against.</param>
        /// <returns>The evaluated value as an untyped object.</returns>
        /// <remarks>
        /// This implementation calls the typed <see cref="EvaluateCore"/> method
        /// and returns the result as an object. Subclasses should override
        /// <see cref="EvaluateCore"/> to provide type-specific evaluation logic.
        /// </remarks>
        public sealed override object Evaluate(object context)
        {
            return EvaluateCore(context);
        }

        /// <summary>
        /// Evaluates the expression against the specified context object with type-safe return.
        /// </summary>
        /// <param name="context">The context object to evaluate against.</param>
        /// <returns>The evaluated value as the specified type.</returns>
        /// <remarks>
        /// Explicit interface implementation for <see cref="IExpressionEvaluator{TResult}"/>.
        /// This implementation calls <see cref="EvaluateCore"/> to perform the actual evaluation.
        /// </remarks>
        TResult IExpressionEvaluator<TResult>.Evaluate(object context)
        {
            return EvaluateCore(context);
        }

        /// <summary>
        /// Evaluates the expression against the specified context object.
        /// </summary>
        /// <param name="context">The context object to evaluate against.</param>
        /// <returns>The evaluated value as the specified type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the evaluator has a validation error.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the evaluated value cannot be cast to <typeparamref name="TResult"/>.
        /// </exception>
        /// <remarks>
        /// Subclasses must implement this method to provide type-specific evaluation logic.
        /// This method is called by the public <see cref="Evaluate(object)"/> methods.
        /// </remarks>
        protected abstract TResult EvaluateCore(object context);
    }
}

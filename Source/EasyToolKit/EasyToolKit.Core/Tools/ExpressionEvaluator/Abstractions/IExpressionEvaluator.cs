using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a value evaluator that computes values from expression paths.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Expression evaluators parse string-based expression paths and compile them
    /// into efficient getter functions for runtime value retrieval. This enables
    /// dynamic member access similar to data binding in UI frameworks.
    /// </para>
    /// <para>
    /// <b>Expression Path Syntax:</b>
    /// <list type="table">
    /// <listheader>
    /// <term>Syntax</term>
    /// <description>Example</description>
    /// <description>Meaning</description>
    /// </listheader>
    /// <item>
    /// <term>Direct member</term>
    /// <description>"PlayerName"</description>
    /// <description>Accesses a field or property named PlayerName</description>
    /// </item>
    /// <item>
    /// <term>Nested access</term>
    /// <description>"Player.Health.CurrentValue"</description>
    /// <description>Accesses nested properties using dot notation</description>
    /// </item>
    /// <item>
    /// <term>Method call</term>
    /// <description>"Inventory.GetItem(0)"</description>
    /// <description>Calls methods and continues the path</description>
    /// </item>
    /// <item>
    /// <term>Static member</term>
    /// <description>"-t:GameConfig -p:MaxLevel"</description>
    /// <description>Accesses static member MaxLevel on type GameConfig</description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IExpressionEvaluator
    {
        /// <summary>
        /// Gets whether this evaluator has a validation error.
        /// </summary>
        /// <param name="errorMessage">The error message, if any.</param>
        /// <returns>
        /// <c>true</c> if there is a validation error; otherwise, <c>false</c>.
        /// </returns>
        bool TryGetError(out string errorMessage);

        /// <summary>
        /// Evaluates the expression against the specified context object.
        /// </summary>
        /// <param name="context">The context object to evaluate against.</param>
        /// <returns>The evaluated value as an untyped object.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the evaluator has a validation error.
        /// </exception>
        object Evaluate(object context);
    }

    /// <summary>
    /// Defines a typed value evaluator that computes values from expression paths.
    /// </summary>
    /// <typeparam name="TResult">The type of value to evaluate.</typeparam>
    /// <remarks>
    /// This interface provides type-safe evaluation with automatic casting.
    /// Use the non-generic <see cref="IExpressionEvaluator"/> interface when the
    /// result type is not known at compile time.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a typed evaluator
    /// var evaluator = ExpressionEvaluatorFactory
    ///     .Evaluate&lt;string&gt;("Player.Name", typeof(Player))
    ///     .Build();
    ///
    /// // Type-safe evaluation
    /// string name = evaluator.Evaluate(player);
    /// </code>
    /// </example>
    public interface IExpressionEvaluator<out TResult> : IExpressionEvaluator
    {
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
        new TResult Evaluate(object context);
    }
}

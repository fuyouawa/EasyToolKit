using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides a base implementation for message dispatchers.
    /// </summary>
    public abstract class MessageDispatcherBase : IMessageDispatcher
    {
        /// <summary>
        /// The target object that receives messages.
        /// </summary>
        protected readonly object Target;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcherBase"/> class.
        /// </summary>
        /// <param name="target">The target object.</param>
        protected MessageDispatcherBase(object target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        /// <summary>
        /// Dispatches a message to all registered handlers.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// True if at least one handler was invoked, false otherwise.
        /// </returns>
        public abstract bool Dispatch(string messageName, object args = null);
    }

    /// <summary>
    /// Provides a base implementation for typed message dispatchers.
    /// </summary>
    /// <typeparam name="TResult">The return type of handlers.</typeparam>
    public abstract class MessageDispatcherBase<TResult> : MessageDispatcherBase, IMessageDispatcher<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcherBase{TResult}"/> class.
        /// </summary>
        /// <param name="target">The target object.</param>
        protected MessageDispatcherBase(object target) : base(target)
        {
        }

        /// <summary>
        /// Dispatches a message to all registered handlers.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// True if at least one handler was invoked, false otherwise.
        /// </returns>
        public sealed override bool Dispatch(string messageName, object args = null)
        {
            return TryDispatch(messageName, args, out _);
        }

        /// <summary>
        /// Dispatches a message and returns the last handler's result.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// The return value from the last handler, or default if no handlers were invoked.
        /// </returns>
        TResult IMessageDispatcher<TResult>.Dispatch(string messageName, object args)
        {
            TryDispatch(messageName, args, out var result);
            return result;
        }

        /// <summary>
        /// Attempts to dispatch a message and returns the result.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <param name="result">The result from the last handler.</param>
        /// <returns>
        /// True if at least one handler was invoked, false otherwise.
        /// </returns>
        protected abstract bool TryDispatch(string messageName, object args, out TResult result);
    }
}

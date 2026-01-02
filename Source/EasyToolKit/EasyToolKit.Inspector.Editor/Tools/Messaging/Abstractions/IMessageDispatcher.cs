namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines a message dispatcher that invokes handlers based on message names.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Dispatches a message to all registered handlers.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// True if at least one handler was invoked, false otherwise.
        /// </returns>
        bool Dispatch(string messageName, object args = null);
    }

    /// <summary>
    /// Defines a typed message dispatcher that returns the last handler's result.
    /// </summary>
    /// <typeparam name="TResult">The return type of handlers.</typeparam>
    public interface IMessageDispatcher<out TResult> : IMessageDispatcher
    {
        /// <summary>
        /// Dispatches a message and returns the last handler's result.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// The return value from the last handler, or default if no handlers were invoked.
        /// </returns>
        TResult Dispatch(string messageName, object args = null);
    }
}

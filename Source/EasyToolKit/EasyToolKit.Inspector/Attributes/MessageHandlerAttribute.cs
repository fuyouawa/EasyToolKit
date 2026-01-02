using System;

namespace EasyToolKit.Inspector
{
    /// <summary>
    /// Marks a method as a message handler for the message dispatcher system.
    /// Handlers are invoked when the specified message name is sent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class MessageHandlerAttribute : Attribute
    {
        /// <summary>
        /// Gets the message name that this method responds to.
        /// If null or empty, uses the method name as the message name.
        /// </summary>
        public string MessageName { get; }

        /// <summary>
        /// Gets the priority of this handler. Higher values are invoked first.
        /// Default is 0. Handlers with the same priority are invoked in declaration order.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
        /// </summary>
        /// <param name="messageName">The message name. If null or empty, uses method name.</param>
        /// <param name="priority">The priority. Higher values invoked first. Default is 0.</param>
        public MessageHandlerAttribute(string messageName = null, int priority = 0)
        {
            MessageName = messageName;
            Priority = priority;
        }
    }
}

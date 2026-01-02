using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides factory methods for creating message dispatchers.
    /// </summary>
    public static class MessageDispatcherFactory
    {
        /// <summary>
        /// Creates a message dispatcher that uses reflection to find handlers.
        /// </summary>
        /// <param name="target">The target object that will receive messages.</param>
        /// <returns>A message dispatcher instance.</returns>
        public static IMessageDispatcher Create(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new AttributeMessageDispatcher(target);
        }

        /// <summary>
        /// Creates a typed message dispatcher that returns the last handler's result.
        /// </summary>
        /// <typeparam name="TResult">The return type to collect.</typeparam>
        /// <param name="target">The target object.</param>
        /// <returns>A typed message dispatcher instance.</returns>
        public static IMessageDispatcher<TResult> Create<TResult>(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new AttributeMessageDispatcher<TResult>(target);
        }
    }
}

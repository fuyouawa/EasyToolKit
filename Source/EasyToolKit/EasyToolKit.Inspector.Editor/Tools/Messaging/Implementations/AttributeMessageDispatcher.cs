using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Message dispatcher that uses reflection to find handler methods marked with
    /// <see cref="MessageHandlerAttribute"/>.
    /// </summary>
    public sealed class AttributeMessageDispatcher : MessageDispatcherBase
    {
        private readonly Dictionary<string, List<HandlerInfo>> _handlers;
        private static readonly Dictionary<Type, Dictionary<string, List<HandlerInfo>>> TypeCache
            = new Dictionary<Type, Dictionary<string, List<HandlerInfo>>>();
        private static readonly object CacheLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeMessageDispatcher"/> class.
        /// </summary>
        /// <param name="target">The target object.</param>
        public AttributeMessageDispatcher(object target) : base(target)
        {
            _handlers = GetOrCreateHandlers(target.GetType());
        }

        internal static Dictionary<string, List<HandlerInfo>> GetOrCreateHandlers(Type targetType)
        {
            // Double-checked locking pattern for thread-safe lazy initialization
            if (TypeCache.TryGetValue(targetType, out var cachedHandlers))
            {
                return cachedHandlers;
            }

            lock (CacheLock)
            {
                // Check again inside the lock
                if (TypeCache.TryGetValue(targetType, out cachedHandlers))
                {
                    return cachedHandlers;
                }

                // Create and cache
                cachedHandlers = ScanType(targetType);
                TypeCache[targetType] = cachedHandlers;
                return cachedHandlers;
            }
        }

        private static Dictionary<string, List<HandlerInfo>> ScanType(Type targetType)
        {
            var handlers = new Dictionary<string, List<HandlerInfo>>();

            // Scan target and its base types for all methods
            foreach (var method in GetAllMethods(targetType))
            {
                var attr = method.GetCustomAttribute<MessageHandlerAttribute>();
                if (attr == null)
                {
                    continue;
                }

                var messageName = string.IsNullOrEmpty(attr.MessageName)
                    ? method.Name
                    : attr.MessageName;

                if (!handlers.TryGetValue(messageName, out var list))
                {
                    list = new List<HandlerInfo>();
                    handlers[messageName] = list;
                }

                list.Add(new HandlerInfo(method, attr.Priority));
            }

            // Sort handlers by priority (descending) and then by metadata token (ascending)
            foreach (var list in handlers.Values)
            {
                list.Sort((a, b) =>
                {
                    var priorityDiff = b.Priority.CompareTo(a.Priority);
                    return priorityDiff != 0
                        ? priorityDiff
                        : a.MethodMetadataToken.CompareTo(b.MethodMetadataToken);
                });
            }

            return handlers;
        }

        /// <summary>
        /// Gets all methods (including inherited) for the specified type.
        /// </summary>
        /// <param name="type">The type to scan.</param>
        /// <returns>All methods in the type hierarchy.</returns>
        private static IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            var methods = new List<MethodInfo>();
            var currentType = type;

            while (currentType != null && currentType != typeof(object))
            {
                // Get all declared methods (instance methods only, no static)
                var typeMethods = currentType.GetMethods(
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                methods.AddRange(typeMethods);
                currentType = currentType.BaseType;
            }

            return methods;
        }

        /// <summary>
        /// Dispatches a message to all registered handlers.
        /// </summary>
        /// <param name="messageName">The message name to dispatch.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// True if at least one handler was invoked, false otherwise.
        /// </returns>
        public override bool Dispatch(string messageName, object args = null)
        {
            if (!_handlers.TryGetValue(messageName, out var handlers))
            {
                return false;
            }

            foreach (var handler in handlers)
            {
                handler.Invoke(Target, args);
            }

            return true;
        }
    }

    /// <summary>
    /// Typed version that returns the last handler's result.
    /// </summary>
    /// <typeparam name="TResult">The return type of handlers.</typeparam>
    public sealed class AttributeMessageDispatcher<TResult> : MessageDispatcherBase<TResult>
    {
        private readonly Dictionary<string, List<HandlerInfo>> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeMessageDispatcher{TResult}"/> class.
        /// </summary>
        /// <param name="target">The target object.</param>
        public AttributeMessageDispatcher(object target) : base(target)
        {
            _handlers = AttributeMessageDispatcher.GetOrCreateHandlers(target.GetType());
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
        protected override bool TryDispatch(string messageName, object args, out TResult result)
        {
            if (!_handlers.TryGetValue(messageName, out var handlers))
            {
                result = default;
                return false;
            }

            result = default;
            foreach (var handler in handlers)
            {
                var handlerResult = handler.Invoke(Target, args);
                if (handlerResult != null && handlerResult is TResult typedResult)
                {
                    result = typedResult;
                }
            }

            return true;
        }
    }
}

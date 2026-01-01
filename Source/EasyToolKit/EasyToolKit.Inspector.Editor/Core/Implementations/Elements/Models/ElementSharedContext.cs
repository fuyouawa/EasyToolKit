using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents a queued event for batch processing.
    /// </summary>
    internal readonly struct QueuedEvent
    {
        public readonly object Sender;
        public readonly EventArgs EventArgs;
        public readonly Delegate Handler;

        public QueuedEvent(object sender, EventArgs eventArgs, Delegate handler)
        {
            Sender = sender;
            EventArgs = eventArgs;
            Handler = handler;
        }
    }

    /// <summary>
    /// Provides shared context for elements within an element tree. Each tree has a single instance
    /// that all elements reference for accessing tree-level services and resolver factories.
    /// </summary>
    public sealed class ElementSharedContext : IElementSharedContext, IDisposable
    {
        private readonly IElementTree _tree;
        private readonly IServiceContainer _serviceContainer;
        private readonly EventHandlerList _eventHandlers;
        private bool _disposed;

        // Batch processing support
        private int _batchDepth;
        private readonly Queue<QueuedEvent> _eventQueue;

        // Compiled invoker cache for each EventArgs type
        private readonly Dictionary<Type, Action<Delegate, object, EventArgs>> _invokerCache;

        /// <summary>
        /// Initializes a new instance with the specified tree and service container.
        /// </summary>
        /// <param name="tree">The element tree this context belongs to.</param>
        /// <param name="serviceContainer">The dependency injection container for resolver factories.</param>
        public ElementSharedContext(IElementTree tree, [CanBeNull] IServiceContainer serviceContainer = null)
        {
            _tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _serviceContainer = serviceContainer ?? CreateDefaultServiceContainer();
            _eventHandlers = new EventHandlerList();
            _eventQueue = new Queue<QueuedEvent>();
            _invokerCache = new Dictionary<Type, Action<Delegate, object, EventArgs>>();
        }

        /// <summary>
        /// Gets the default service descriptors for all resolver factories.
        /// </summary>
        /// <returns>A collection of default service descriptors for resolver factories.</returns>
        public static IEnumerable<ServiceDescriptor> GetDefaultServiceDescriptors()
        {
            yield return ServiceDescriptor.Singleton<IResolverFactory<IAttributeResolver>, DefaultAttributeResolverFactory>();
            yield return ServiceDescriptor.Singleton<IResolverFactory<IStructureResolver>, DefaultStructureResolverFactory>();
            yield return ServiceDescriptor.Singleton<IResolverFactory<IValueOperationResolver>, DefaultValueOperationResolverFactory>();
            yield return ServiceDescriptor.Singleton<IResolverFactory<IDrawerChainResolver>, DefaultDrawerChainResolverFactory>();
            yield return ServiceDescriptor.Singleton<IResolverFactory<IPostProcessorChainResolver>, DefaultPostProcessorChainResolverFactory>();
        }

        /// <summary>
        /// Creates a default service container with all standard resolver factories registered.
        /// </summary>
        /// <returns>A new service container with default resolver factories registered.</returns>
        public static IServiceContainer CreateDefaultServiceContainer()
        {
            return ServiceContainerBuilder.Build(GetDefaultServiceDescriptors());
        }

        public int UpdateId => _tree.UpdateId;

        public IElementTree Tree => _tree;

        /// <summary>
        /// Gets a value indicating whether events are currently being batched.
        /// When true, events are queued and will be triggered in batch when <see cref="EndBatchMode"/> is called.
        /// </summary>
        public bool IsInBatchMode => _batchDepth > 0;

        public void RegisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            ValidateDisposed();
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventHandlers.AddHandler(typeof(TEventArgs), handler);
        }

        public void UnregisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            ValidateDisposed();
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventHandlers.RemoveHandler(typeof(TEventArgs), handler);
        }

        public void TriggerEvent<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            ValidateDisposed();
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var handler = _eventHandlers[typeof(TEventArgs)];
            if (handler == null)
                return;

            if (_batchDepth > 0)
            {
                // Queue the event for batch processing
                _eventQueue.Enqueue(new QueuedEvent(sender, eventArgs, handler));
            }
            else
            {
                // Trigger immediately with optimized invoke
                InvokeEventHandler(handler, sender, eventArgs);
            }
        }

        /// <summary>
        /// Begins batch mode. Events will be queued until <see cref="EndBatchMode"/> is called.
        /// Supports nested batching with reference counting.
        /// </summary>
        public void BeginBatchMode()
        {
            ValidateDisposed();
            _batchDepth++;
        }

        /// <summary>
        /// Ends batch mode and flushes all queued events. If this is the outermost batch level,
        /// all queued events will be triggered in order.
        /// </summary>
        public void EndBatchMode()
        {
            ValidateDisposed();
            if (_batchDepth <= 0)
            {
                throw new InvalidOperationException("Batch mode is not active. Call BeginBatchMode first.");
            }

            _batchDepth--;
            if (_batchDepth == 0 && _eventQueue.Count > 0)
            {
                FlushEventQueue();
            }
        }

        /// <summary>
        /// Invokes an event handler with optimized casting instead of DynamicInvoke.
        /// </summary>
        private void InvokeEventHandler<TEventArgs>(Delegate handler, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            // Use direct cast and invoke instead of DynamicInvoke for better performance
            if (handler is EventHandler<TEventArgs> typedHandler)
            {
                typedHandler.Invoke(sender, eventArgs);
            }
            else
            {
                // Fallback to DynamicInvoke for unexpected delegate types
                handler?.DynamicInvoke(sender, eventArgs);
            }
        }

        /// <summary>
        /// Compiles an optimized invoker delegate for the specified event args type using expression trees.
        /// The compiled delegate avoids the overhead of DynamicInvoke and type-specific if-else chains.
        /// </summary>
        /// <param name="eventArgsType">The type of event arguments to compile an invoker for.</param>
        /// <returns>A compiled delegate that invokes the handler with proper type casting.</returns>
        private Action<Delegate, object, EventArgs> CompileInvoker(Type eventArgsType)
        {
            // Create parameters: handler, sender, eventArgs
            var handlerParam = Expression.Parameter(typeof(Delegate), "handler");
            var senderParam = Expression.Parameter(typeof(object), "sender");
            var eventArgsParam = Expression.Parameter(typeof(EventArgs), "eventArgs");

            // Create the typed EventHandler<TEventArgs> type
            var typedHandlerType = typeof(EventHandler<>).MakeGenericType(eventArgsType);

            // Cast handler to EventHandler<TEventArgs>
            var convertHandler = Expression.Convert(handlerParam, typedHandlerType);

            // Cast eventArgs to the specific event args type
            var convertEventArgs = Expression.Convert(eventArgsParam, eventArgsType);

            // Get the Invoke method from EventHandler<TEventArgs>
            var invokeMethod = typedHandlerType.GetMethod("Invoke");

            // Build the expression: ((EventHandler<TEventArgs>)handler).Invoke(sender, (TEventArgs)eventArgs)
            var invokeCall = Expression.Call(convertHandler, invokeMethod, senderParam, convertEventArgs);

            // Compile the expression tree into a delegate
            return Expression.Lambda<Action<Delegate, object, EventArgs>>(
                invokeCall,
                handlerParam,
                senderParam,
                eventArgsParam
            ).Compile();
        }

        /// <summary>
        /// Flushes all queued events and triggers them in order.
        /// Uses compiled invokers for optimal performance without type-specific if-else chains.
        /// </summary>
        private void FlushEventQueue()
        {
            while (_eventQueue.Count > 0)
            {
                var queuedEvent = _eventQueue.Dequeue();
                var eventArgsType = queuedEvent.EventArgs.GetType();

                // Get or compile invoker for this event args type
                if (!_invokerCache.TryGetValue(eventArgsType, out var invoker))
                {
                    invoker = CompileInvoker(eventArgsType);
                    _invokerCache[eventArgsType] = invoker;
                }

                // Invoke using the compiled delegate
                invoker(queuedEvent.Handler, queuedEvent.Sender, queuedEvent.EventArgs);
            }
        }

        public IResolverFactory<TResolver> GetResolverFactory<TResolver>() where TResolver : IResolver
        {
            ValidateDisposed();
            var factoryType = typeof(IResolverFactory<TResolver>);
            var factory = _serviceContainer.GetService(factoryType);

            if (factory == null)
            {
                throw new InvalidOperationException(
                    $"No resolver factory of type '{factoryType.FullName}' has been registered in the service container.");
            }

            if (!(factory is IResolverFactory<TResolver> typedFactory))
            {
                throw new InvalidOperationException(
                    $"The registered service for type '{factoryType.FullName}' is not an instance of '{factoryType.FullName}'.");
            }

            return typedFactory;
        }

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
                return;

            _serviceContainer.Dispose();
            _disposed = true;
        }
    }
}

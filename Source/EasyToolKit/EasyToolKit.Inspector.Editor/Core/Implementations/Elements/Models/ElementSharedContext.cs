using System;
using System.Collections.Generic;
using System.ComponentModel;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides shared context for elements within an element tree. Each tree has a single instance
    /// that all elements reference for accessing tree-level services and resolver factories.
    /// </summary>
    public sealed class ElementSharedContext : IElementSharedContext
    {
        private readonly IElementTree _tree;
        private readonly IServiceContainer _serviceContainer;
        private readonly EventHandlerList _eventHandlers;
        private bool _disposed;

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

        public void TriggerEvent(object sender, EventArgs eventArgs)
        {
            ValidateDisposed();
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var eventType = eventArgs.GetType();
            var handler = _eventHandlers[eventType];
            handler?.DynamicInvoke(sender, eventArgs);
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

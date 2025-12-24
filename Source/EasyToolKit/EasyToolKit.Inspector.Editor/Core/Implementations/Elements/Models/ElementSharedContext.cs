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
        private readonly IServiceContainer _serviceContainer;
        private readonly EventHandlerList _eventHandlers;

        /// <summary>
        /// Initializes a new instance with the specified tree and service container.
        /// </summary>
        /// <param name="tree">The element tree this context belongs to.</param>
        /// <param name="serviceContainer">The dependency injection container for resolver factories.</param>
        public ElementSharedContext(IElementTree tree, [CanBeNull] IServiceContainer serviceContainer = null)
        {
            Tree = tree ?? throw new ArgumentNullException(nameof(tree));
            _serviceContainer = serviceContainer ?? CreateDefaultServiceContainer();
            _eventHandlers = new EventHandlerList();
        }

        /// <summary>
        /// Gets the default service descriptors for all resolver factories.
        /// </summary>
        /// <returns>A collection of default service descriptors for resolver factories.</returns>
        public static IEnumerable<ServiceDescriptor> GetDefaultServiceDescriptors()
        {
            yield return ServiceDescriptor.Singleton<IAttributeResolverFactory, DefaultAttributeResolverFactory>();
            yield return ServiceDescriptor.Singleton<IValueStructureResolverFactory, DefaultValueStructureResolverFactory>();
            yield return ServiceDescriptor.Singleton<IValueOperationResolverFactory, DefaultValueOperationResolverFactory>();
            yield return ServiceDescriptor.Singleton<IDrawerChainResolverFactory, DefaultDrawerChainResolverFactory>();
            yield return ServiceDescriptor.Singleton<IPostProcessorChainResolverFactory, DefaultPostProcessorChainResolverFactory>();
        }

        /// <summary>
        /// Creates a default service container with all standard resolver factories registered.
        /// </summary>
        /// <returns>A new service container with default resolver factories registered.</returns>
        public static IServiceContainer CreateDefaultServiceContainer()
        {
            return ServiceContainerBuilder.Build(GetDefaultServiceDescriptors());
        }

        public int UpdateId => Tree.UpdateId;

        public IElementTree Tree { get; }

        public void RegisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventHandlers.AddHandler(typeof(TEventArgs), handler);
        }

        public void UnregisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _eventHandlers.RemoveHandler(typeof(TEventArgs), handler);
        }

        public void TriggerEvent(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            var eventType = eventArgs.GetType();
            var handler = _eventHandlers[eventType] as EventHandler;
            handler?.Invoke(sender, eventArgs);
        }

        public IResolverFactory<TResolver> GetResolverFactory<TResolver>() where TResolver : IResolver
        {
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

        void IDisposable.Dispose()
        {
            _serviceContainer.Dispose();
        }
    }
}

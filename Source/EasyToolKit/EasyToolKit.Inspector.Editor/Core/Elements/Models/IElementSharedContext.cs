using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the shared context for elements within an element tree. Each tree has a single ElementSharedContext instance,
    /// and each IElement holds a reference to this shared context. The context provides access to tree-level
    /// services and resolver factories using dependency injection.
    /// </summary>
    public interface IElementSharedContext
    {
        /// <summary>
        /// Gets the update identifier that increments every frame.
        /// This value corresponds to <see cref="IElementTree.UpdateId"/> and is used to prevent certain logic
        /// from executing multiple times within a single frame, ensuring that operations which should only
        /// run once per frame are properly controlled.
        /// </summary>
        int UpdateId { get; }

        /// <summary>
        /// Gets a reference to the element tree that this context belongs to.
        /// </summary>
        IElementTree Tree { get; }

        void RegisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs;

        void UnregisterEventHandler<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : EventArgs;

        void TriggerEvent<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs;

        /// <summary>
        /// Begins batch mode. Events will be queued until <see cref="EndBatchMode"/> is called.
        /// Supports nested batching with reference counting. This is useful for operations that trigger
        /// many events in succession, such as post-processing nested element hierarchies.
        /// </summary>
        void BeginBatchMode();

        /// <summary>
        /// Ends batch mode and flushes all queued events. If this is the outermost batch level,
        /// all queued events will be triggered in order using optimized invocation.
        /// </summary>
        void EndBatchMode();

        /// <summary>
        /// Gets a value indicating whether events are currently being batched.
        /// When true, events are queued and triggered in batch via <see cref="EndBatchMode"/>.
        /// </summary>
        bool IsInBatchMode { get; }

        /// <summary>
        /// Gets the resolver factory of the specified type from the internal dependency injection container.
        /// Resolver factories are used to create resolvers for different element types and operations.
        /// </summary>
        /// <typeparam name="TResolver">The type of resolver to retrieve. Must implement <see cref="IResolver"/>.</typeparam>
        /// <returns>The resolver factory instance of the specified type.</returns>
        IResolverFactory<TResolver> GetResolverFactory<TResolver>() where TResolver : IResolver;
    }
}

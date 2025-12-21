using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;

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

        /// <summary>
        /// Gets the resolver factory of the specified type from the internal dependency injection container.
        /// Resolver factories are used to create resolvers for different element types and operations.
        /// </summary>
        /// <typeparam name="T">The type of resolver factory to retrieve. Must implement <see cref="IResolverFactory"/>.</typeparam>
        /// <returns>The resolver factory instance of the specified type.</returns>
        T GetResolverFactory<T>() where T : IResolverFactory;
    }
}
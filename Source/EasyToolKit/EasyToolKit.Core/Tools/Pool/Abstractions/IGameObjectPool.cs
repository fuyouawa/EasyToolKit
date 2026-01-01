using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a specialized object pool for Unity GameObject instances.
    /// </summary>
    public interface IGameObjectPool : IPool<GameObject>
    {
        /// <summary>
        /// Gets the original prefab used for instantiation.
        /// </summary>
        GameObject Original { get; }

        /// <summary>
        /// Gets the Transform component for managing pooled object hierarchy.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Gets the lifetime accessor for a specific pooled GameObject.
        /// </summary>
        /// <param name="instance">The pooled GameObject instance.</param>
        /// <returns>The lifetime accessor for the instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="instance"/> is not managed by this pool.
        /// </exception>
        IPooledGameObjectLifetimeAccessor GetLifetimeAccessor(GameObject instance);

        /// <summary>
        /// Tries to get the lifetime accessor for a specific pooled GameObject.
        /// </summary>
        /// <param name="instance">The pooled GameObject instance.</param>
        /// <param name="lifetimeAccessor">
        /// When this method returns, contains the lifetime accessor if found;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the lifetime accessor was found;
        /// <c>false</c> if the instance is not managed by this pool.
        /// </returns>
        bool TryGetLifetimeAccessor(GameObject instance, out IPooledGameObjectLifetimeAccessor lifetimeAccessor);

        /// <summary>
        /// Updates the pool state, processing object lifetimes and recycling.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last update (in seconds).</param>
        void Tick(float deltaTime);
    }
}

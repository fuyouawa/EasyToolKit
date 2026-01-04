using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a manager for creating and managing GameObject pools.
    /// </summary>
    public interface IGameObjectPoolManager
    {
        /// <summary>
        /// Gets the root Transform for pooled GameObject hierarchy.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Gets the names of all managed pools.
        /// </summary>
        IEnumerable<string> GetPoolNames();

        /// <summary>
        /// Determines whether a pool with the specified name exists.
        /// </summary>
        /// <param name="poolName">The name of the pool.</param>
        /// <returns>
        /// <c>true</c> if a pool with the specified name exists; otherwise, <c>false</c>.
        /// </returns>
        bool HasPool(string poolName);

        /// <summary>
        /// Tries to get the pool with the specified name.
        /// </summary>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="pool">
        /// When this method returns, contains the pool if found;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the pool was found; otherwise, <c>false</c>.
        /// </returns>
        bool TryGetPool(string poolName, out IGameObjectPool pool);

        /// <summary>
        /// Gets the pool with the specified name.
        /// </summary>
        /// <param name="poolName">The name of the pool.</param>
        /// <returns>The pool with the specified name.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a pool with the specified name does not exist.
        /// </exception>
        IGameObjectPool GetPool(string poolName);

        /// <summary>
        /// Creates a builder for configuring and creating a new GameObject pool.
        /// </summary>
        /// <param name="poolName">The name of the pool.</param>
        /// <param name="original">The original prefab for instantiation.</param>
        /// <returns>A builder for configuring the pool.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="poolName"/> or <paramref name="original"/> is null.
        /// </exception>
        IGameObjectPoolBuilder BuildPool(string poolName, GameObject original);
    }
}

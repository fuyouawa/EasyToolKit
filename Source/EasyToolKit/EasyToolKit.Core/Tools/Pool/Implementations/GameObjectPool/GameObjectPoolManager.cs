using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Implementation of a manager for creating and managing GameObject pools.
    /// </summary>
    public sealed class GameObjectPoolManager : IGameObjectPoolManager, IGameObjectPoolTicker
    {
        private readonly Dictionary<string, IGameObjectPool> _pools =
            new Dictionary<string, IGameObjectPool>();

        private readonly Transform _rootTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectPoolManager"/> class.
        /// </summary>
        /// <param name="rootTransform">The root Transform for pooled GameObject hierarchy.</param>
        internal GameObjectPoolManager(Transform rootTransform)
        {
            _rootTransform = rootTransform;
        }

        /// <inheritdoc />
        public Transform Transform => _rootTransform;

        /// <inheritdoc />
        public IEnumerable<string> GetPoolNames()
        {
            return _pools.Keys;
        }

        /// <inheritdoc />
        public bool HasPool(string poolName)
        {
            if (poolName == null)
            {
                throw new ArgumentNullException(nameof(poolName));
            }

            return _pools.ContainsKey(poolName);
        }

        /// <inheritdoc />
        public bool TryGetPool(string poolName, out IGameObjectPool pool)
        {
            if (poolName == null)
            {
                throw new ArgumentNullException(nameof(poolName));
            }

            return _pools.TryGetValue(poolName, out pool);
        }

        /// <inheritdoc />
        public IGameObjectPool GetPool(string poolName)
        {
            if (poolName == null)
            {
                throw new ArgumentNullException(nameof(poolName));
            }

            if (_pools.TryGetValue(poolName, out var pool))
            {
                return pool;
            }

            throw new InvalidOperationException(
                $"Game object pool '{poolName}' does not exist.");
        }

        /// <inheritdoc />
        public IGameObjectPoolBuilder BuildPool(string poolName, GameObject original)
        {
            if (poolName == null)
            {
                throw new ArgumentNullException(nameof(poolName));
            }

            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            return new GameObjectPoolBuilder(poolName, original, this);
        }

        /// <inheritdoc />
        public void Tick(float deltaTime)
        {
            foreach (var pool in _pools.Values)
            {
                pool.Tick(deltaTime);
            }
        }

        internal IGameObjectPool CreatePoolFromBuilder(string poolName, GameObject original, IGameObjectPoolDefinition definition)
        {
            if (_pools.ContainsKey(poolName))
            {
                throw new InvalidOperationException(
                    $"Game object pool '{poolName}' already exists.");
            }

            // Create a root GameObject for this pool
            var poolRoot = new GameObject(poolName);
            poolRoot.transform.SetParent(_rootTransform, false);

            var pool = new GameObjectPool(poolName, original, definition, poolRoot.transform);
            _pools.Add(poolName, pool);

            return pool;
        }
    }
}

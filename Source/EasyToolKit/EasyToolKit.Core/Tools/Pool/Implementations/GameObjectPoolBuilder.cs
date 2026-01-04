using UnityEngine;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Builder for configuring and creating GameObject pools.
    /// </summary>
    public sealed class GameObjectPoolBuilder : IGameObjectPoolBuilder
    {
        private readonly string _poolName;
        private readonly GameObject _original;
        private readonly IGameObjectPoolManager _manager;

        /// <inheritdoc />
        public int InitialCapacity { get; set; }

        /// <inheritdoc />
        public int MaxCapacity { get; set; } = -1;

        /// <inheritdoc />
        public bool CallPoolItemCallbacks { get; set; } = true;

        /// <inheritdoc />
        public float DefaultActiveLifetime { get; set; }

        /// <inheritdoc />
        public float DefaultIdleLifetime { get; set; } = 10f;

        /// <inheritdoc />
        public float TickInterval { get; set; } = 0.5f;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectPoolBuilder"/> class.
        /// </summary>
        /// <param name="poolName">The name for the pool.</param>
        /// <param name="original">The original prefab for instantiation.</param>
        /// <param name="manager">The manager that will own this pool.</param>
        internal GameObjectPoolBuilder(string poolName, GameObject original, IGameObjectPoolManager manager)
        {
            _poolName = poolName;
            _original = original;
            _manager = manager;
        }

        /// <inheritdoc />
        public IGameObjectPool Create()
        {
            var definition = new GameObjectPoolDefinition
            {
                InitialCapacity = InitialCapacity,
                MaxCapacity = MaxCapacity,
                CallPoolItemCallbacks = CallPoolItemCallbacks,
                DefaultActiveLifetime = DefaultActiveLifetime,
                DefaultIdleLifetime = DefaultIdleLifetime,
                TickInterval = TickInterval
            };

            return ((GameObjectPoolManager)_manager).CreatePoolFromBuilder(_poolName, _original, definition);
        }
    }
}

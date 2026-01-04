using System;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Builder for configuring and creating object pools.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public sealed class ObjectPoolBuilder<T> : IObjectPoolBuilder<T> where T : class, new()
    {
        private readonly string _poolName;
        private readonly IObjectPoolManager _manager;

        /// <inheritdoc />
        public int InitialCapacity { get; set; }

        /// <inheritdoc />
        public int MaxCapacity { get; set; } = -1;

        /// <inheritdoc />
        public bool CallPoolItemCallbacks { get; set; } = true;

        /// <inheritdoc />
        public Func<T> Factory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPoolBuilder{T}"/> class.
        /// </summary>
        /// <param name="poolName">The name for the pool.</param>
        /// <param name="manager">The manager that will own this pool.</param>
        internal ObjectPoolBuilder(string poolName, IObjectPoolManager manager)
        {
            _poolName = poolName;
            _manager = manager;
        }

        /// <inheritdoc />
        public IObjectPool<T> Create()
        {
            var definition = new ObjectPoolDefinition<T>
            {
                InitialCapacity = InitialCapacity,
                MaxCapacity = MaxCapacity,
                CallPoolItemCallbacks = CallPoolItemCallbacks,
                Factory = Factory ?? (() => new T())
            };

            return ((ObjectPoolManager)_manager).CreatePoolFromBuilder(_poolName, definition);
        }
    }
}

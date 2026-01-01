namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Factory implementation for creating pool configuration instances.
    /// Provides methods to create configurations for all supported pool types.
    /// </summary>
    public sealed class PoolConfigurator : IPoolConfigurator
    {
        /// <inheritdoc />
        public IObjectPoolConfiguration<T> ObjectPool<T>() where T : class, new()
        {
            return new ObjectPoolConfiguration<T>();
        }

        /// <inheritdoc />
        public IGameObjectPoolConfiguration GameObjectPool()
        {
            return new GameObjectPoolConfiguration();
        }
    }
}

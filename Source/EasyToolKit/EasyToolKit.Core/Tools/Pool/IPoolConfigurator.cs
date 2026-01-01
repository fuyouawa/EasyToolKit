namespace EasyToolKit.Core
{
    /// <summary>
    /// Factory interface for creating pool configuration instances.
    /// Provides methods to create configurations for all supported pool types.
    /// </summary>
    public interface IPoolConfigurator
    {
        /// <summary>
        /// Creates a new <see cref="IObjectPoolConfiguration{T}"/> instance for C# object pools.
        /// </summary>
        /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
        /// <returns>A new object pool configuration instance.</returns>
        IObjectPoolConfiguration<T> ObjectPool<T>() where T : class, new();

        /// <summary>
        /// Creates a new <see cref="IGameObjectPoolConfiguration"/> instance for GameObject pools.
        /// </summary>
        /// <returns>A new GameObject pool configuration instance.</returns>
        IGameObjectPoolConfiguration GameObjectPool();
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Manages a collection of C# object pools.
    /// </summary>
    public interface IObjectPoolManager
    {
        /// <summary>
        /// Creates a new object pool with the specified name and definition.
        /// </summary>
        /// <typeparam name="T">The type of objects to pool.</typeparam>
        /// <param name="poolName">The unique name for the pool.</param>
        /// <param name="definition">The definition for the pool. If null, default configuration is used.</param>
        /// <returns>The created pool.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a pool with the specified name already exists.
        /// </exception>
        IObjectPool<T> CreatePool<T>(string poolName, IObjectPoolDefinition<T> definition = null)
            where T : class, new();

        /// <summary>
        /// Attempts to get the pool with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="poolName">The name of the pool to retrieve.</param>
        /// <param name="pool">
        /// When this method returns, contains the pool if found;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the pool was found; otherwise, <c>false</c>.</returns>
        bool TryGetPool<T>(string poolName, out IObjectPool<T> pool)
            where T : class, new();
    }
}

using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Manages a collection of named pools.
    /// </summary>
    /// <typeparam name="T">The type of pool instances managed.</typeparam>
    public interface IPoolManager<T> where T : class
    {
        /// <summary>
        /// Gets the number of pools managed by this manager.
        /// </summary>
        int PoolCount { get; }

        /// <summary>
        /// Determines whether a pool with the specified name exists.
        /// </summary>
        /// <param name="poolName">The name of the pool to check.</param>
        /// <returns><c>true</c> if a pool with the specified name exists; otherwise, <c>false</c>.</returns>
        bool HasPool(string poolName);

        /// <summary>
        /// Attempts to get the pool with the specified name.
        /// </summary>
        /// <param name="poolName">The name of the pool to retrieve.</param>
        /// <param name="pool">
        /// When this method returns, contains the pool if found;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the pool was found; otherwise, <c>false</c>.</returns>
        bool TryGetPool(string poolName, out T pool);

        /// <summary>
        /// Gets all pool names managed by this instance.
        /// </summary>
        /// <returns>A collection of pool names.</returns>
        IEnumerable<string> GetPoolNames();
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for pool interfaces.
    /// </summary>
    public static class PoolExtensions
    {
        /// <summary>
        /// Adds a callback to be invoked when an object is rented from the pool.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="pool">The object pool.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>The same pool instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pool"/> or <paramref name="callback"/> is null.</exception>
        public static IObjectPool<T> OnRent<T>(this IObjectPool<T> pool, Action<T> callback) where T : class, new()
        {
            if (pool == null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            pool.AddRentCallback(callback);
            return pool;
        }

        /// <summary>
        /// Adds a callback to be invoked when an object is released back to the pool.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="pool">The object pool.</param>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>The same pool instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pool"/> or <paramref name="callback"/> is null.</exception>
        public static IObjectPool<T> OnRelease<T>(this IObjectPool<T> pool, Action<T> callback) where T : class, new()
        {
            if (pool == null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            pool.AddReleaseCallback(callback);
            return pool;
        }

        /// <summary>
        /// Sets the capacity of a pool.
        /// </summary>
        /// <typeparam name="T">The type of objects in the pool.</typeparam>
        /// <param name="pool">The pool.</param>
        /// <param name="capacity">The new capacity value.</param>
        /// <returns>The same pool instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="pool"/> is null.</exception>
        public static IPool<T> SetCapacity<T>(this IPool<T> pool, int capacity)
        {
            if (pool == null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            pool.Capacity = capacity;
            return pool;
        }
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a generic object pool with type-safe operations.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by this pool.</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets the name of this pool.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the number of objects currently in use (rented).
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// Gets the number of idle objects available for rent.
        /// </summary>
        int IdleCount { get; }

        /// <summary>
        /// Gets or sets the maximum capacity of this pool.
        /// </summary>
        /// <remarks>
        /// Values less than zero indicate unlimited capacity.
        /// When setting a new capacity, if the new value is less than the current active count,
        /// an <see cref="InvalidOperationException"/> will be thrown.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when setting a capacity less than the current active count.
        /// </exception>
        int Capacity { get; set; }

        /// <summary>
        /// Rents an object from the pool.
        /// </summary>
        /// <returns>An object instance from the pool.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the pool has reached its capacity limit and no idle objects are available.
        /// </exception>
        T Rent();

        /// <summary>
        /// Releases an object back to the pool.
        /// </summary>
        /// <param name="instance">The object instance to release.</param>
        /// <returns>
        /// <c>true</c> if the object was successfully released;
        /// <c>false</c> if the object was already idle or does not belong to this pool.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
        bool Release(T instance);

        /// <summary>
        /// Permanently removes an object from the pool.
        /// </summary>
        /// <param name="instance">The object instance to remove.</param>
        /// <returns>
        /// <c>true</c> if the object was successfully removed;
        /// <c>false</c> if the object was not found in this pool.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
        bool Remove(T instance);
    }
}

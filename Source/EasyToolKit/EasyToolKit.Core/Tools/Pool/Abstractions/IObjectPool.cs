using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a pool for managing C# object instances with type safety.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by this pool.</typeparam>
    public interface IObjectPool<T> : IPool<T> where T : class, new()
    {
        /// <summary>
        /// Gets the type of objects managed by this pool.
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// Adds a callback to be invoked when an object is rented from the pool.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        void AddRentCallback(Action<T> callback);

        /// <summary>
        /// Adds a callback to be invoked when an object is released back to the pool.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        void AddReleaseCallback(Action<T> callback);
    }
}

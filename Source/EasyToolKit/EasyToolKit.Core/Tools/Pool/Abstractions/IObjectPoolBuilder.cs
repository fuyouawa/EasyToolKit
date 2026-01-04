using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a builder for configuring and creating object pools.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public interface IObjectPoolBuilder<T> : IPoolBuilder where T : class, new()
    {
        /// <summary>
        /// Gets or sets the factory function for creating new instances.
        /// If null, uses the default constructor.
        /// </summary>
        Func<T> Factory { get; set; }

        /// <summary>
        /// Creates the configured object pool.
        /// </summary>
        /// <returns>The configured object pool instance.</returns>
        IObjectPool<T> Create();
    }
}

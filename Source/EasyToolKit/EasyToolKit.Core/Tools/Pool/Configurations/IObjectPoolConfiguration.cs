using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Configuration interface for creating C# object pool definitions.
    /// Provides mutable builder properties for object pool configuration.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public interface IObjectPoolConfiguration<T> : IPoolConfiguration where T : class, new()
    {
        /// <summary>
        /// Gets or sets the factory function for creating new instances.
        /// If <c>null</c>, uses the default constructor.
        /// </summary>
        Func<T> Factory { get; set; }

        /// <summary>
        /// Creates a new <see cref="IObjectPoolDefinition{T}"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new object pool definition instance.</returns>
        IObjectPoolDefinition<T> CreateDefinition();
    }
}

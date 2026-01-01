using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines the configuration for C# object pools.
    /// Contains immutable data for pool initialization.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public interface IObjectPoolDefinition<T> : IPoolDefinition where T : class, new()
    {
        /// <summary>
        /// Gets the factory function for creating new instances.
        /// If <c>null</c>, uses the default constructor.
        /// </summary>
        Func<T> Factory { get; }
    }
}

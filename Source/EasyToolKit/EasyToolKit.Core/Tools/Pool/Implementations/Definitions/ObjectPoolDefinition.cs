using System;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IObjectPoolDefinition{T}"/>.
    /// Stores immutable configuration data for object pools.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public sealed class ObjectPoolDefinition<T> : IObjectPoolDefinition<T> where T : class, new()
    {
        /// <inheritdoc />
        public int InitialCapacity { get; internal set; }

        /// <inheritdoc />
        public int MaxCapacity { get; internal set; }

        /// <inheritdoc />
        public bool CallPoolItemCallbacks { get; internal set; }

        /// <inheritdoc />
        public Func<T> Factory { get; internal set; }
    }
}

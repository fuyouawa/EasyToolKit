using System;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Configuration implementation for creating object pool definitions.
    /// Provides mutable builder properties and validation for object pools.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public class ObjectPoolConfiguration<T> : IObjectPoolConfiguration<T> where T : class, new()
    {
        /// <inheritdoc />
        public int InitialCapacity { get; set; }

        /// <inheritdoc />
        public int MaxCapacity { get; set; } = -1;

        /// <inheritdoc />
        public bool CallPoolItemCallbacks { get; set; } = true;

        /// <inheritdoc />
        public Func<T> Factory { get; set; }

        /// <summary>
        /// Processes the configuration and validates all properties before creating the definition.
        /// </summary>
        /// <param name="definition">The definition to process.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when validation fails (e.g., InitialCapacity is negative or exceeds MaxCapacity).
        /// </exception>
        protected void ProcessDefinition(ObjectPoolDefinition<T> definition)
        {
            if (InitialCapacity < 0)
            {
                throw new InvalidOperationException(
                    $"InitialCapacity cannot be negative. Current value: {InitialCapacity}");
            }

            if (MaxCapacity >= 0 && InitialCapacity > MaxCapacity)
            {
                throw new InvalidOperationException(
                    $"InitialCapacity ({InitialCapacity}) cannot exceed MaxCapacity ({MaxCapacity})");
            }

            definition.InitialCapacity = InitialCapacity;
            definition.MaxCapacity = MaxCapacity;
            definition.CallPoolItemCallbacks = CallPoolItemCallbacks;
            definition.Factory = Factory ?? (() => new T());
        }

        /// <inheritdoc />
        public IObjectPoolDefinition<T> CreateDefinition()
        {
            var definition = new ObjectPoolDefinition<T>();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

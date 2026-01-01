namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IGameObjectPoolDefinition"/>.
    /// Stores immutable configuration data for GameObject pools.
    /// </summary>
    public sealed class GameObjectPoolDefinition : IGameObjectPoolDefinition
    {
        /// <inheritdoc />
        public int InitialCapacity { get; internal set; }

        /// <inheritdoc />
        public int MaxCapacity { get; internal set; }

        /// <inheritdoc />
        public bool CallPoolItemCallbacks { get; internal set; }

        /// <inheritdoc />
        public float DefaultActiveLifetime { get; internal set; }

        /// <inheritdoc />
        public float DefaultIdleLifetime { get; internal set; }

        /// <inheritdoc />
        public float TickInterval { get; internal set; }
    }
}

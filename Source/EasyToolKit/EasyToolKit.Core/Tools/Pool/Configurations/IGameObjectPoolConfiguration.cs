namespace EasyToolKit.Core
{
    /// <summary>
    /// Configuration interface for creating GameObject pool definitions.
    /// Provides mutable builder properties for GameObject pool configuration.
    /// </summary>
    public interface IGameObjectPoolConfiguration : IPoolConfiguration
    {
        /// <summary>
        /// Gets or sets the default maximum lifetime for active objects.
        /// Objects exceeding this time will be recycled back to the pool.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float DefaultActiveLifetime { get; set; }

        /// <summary>
        /// Gets or sets the default maximum lifetime for idle objects.
        /// Objects exceeding this time will be destroyed.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float DefaultIdleLifetime { get; set; }

        /// <summary>
        /// Gets or sets the interval between tick updates (in seconds).
        /// </summary>
        float TickInterval { get; set; }

        /// <summary>
        /// Creates a new <see cref="IGameObjectPoolDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new GameObject pool definition instance.</returns>
        IGameObjectPoolDefinition CreateDefinition();
    }
}

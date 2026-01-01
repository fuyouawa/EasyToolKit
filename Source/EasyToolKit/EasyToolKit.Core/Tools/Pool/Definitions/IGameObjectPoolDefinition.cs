namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines the configuration for GameObject pools.
    /// Contains immutable data for pool initialization.
    /// </summary>
    public interface IGameObjectPoolDefinition : IPoolDefinition
    {
        /// <summary>
        /// Gets the default maximum lifetime for active objects.
        /// Objects exceeding this time will be recycled back to the pool.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float DefaultActiveLifetime { get; }

        /// <summary>
        /// Gets the default maximum lifetime for idle objects.
        /// Objects exceeding this time will be destroyed.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float DefaultIdleLifetime { get; }

        /// <summary>
        /// Gets the interval between tick updates (in seconds).
        /// </summary>
        float TickInterval { get; }
    }
}

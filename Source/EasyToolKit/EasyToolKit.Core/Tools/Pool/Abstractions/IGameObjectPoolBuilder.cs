namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a builder for configuring and creating GameObject pools.
    /// </summary>
    public interface IGameObjectPoolBuilder : IPoolBuilder
    {
        /// <summary>
        /// Gets or sets the default maximum lifetime for active objects.
        /// </summary>
        float DefaultActiveLifetime { get; set; }

        /// <summary>
        /// Gets or sets the default maximum lifetime for idle objects.
        /// </summary>
        float DefaultIdleLifetime { get; set; }

        /// <summary>
        /// Gets or sets the interval between tick updates.
        /// </summary>
        float TickInterval { get; set; }

        /// <summary>
        /// Creates the configured GameObject pool.
        /// </summary>
        /// <returns>The configured GameObject pool instance.</returns>
        IGameObjectPool Create();
    }
}

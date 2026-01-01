namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IGameObjectPoolConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring GameObject pool properties.
    /// </summary>
    public static class GameObjectPoolConfigurationExtensions
    {
        /// <summary>
        /// Sets the default maximum lifetime for active objects.
        /// </summary>
        /// <typeparam name="TConfiguration">The GameObject pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="lifetime">The lifetime in seconds. Use -1 for unlimited.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithActiveLifetime<TConfiguration>(
            this TConfiguration configuration, float lifetime)
            where TConfiguration : IGameObjectPoolConfiguration
        {
            configuration.DefaultActiveLifetime = lifetime;
            return configuration;
        }

        /// <summary>
        /// Sets the default maximum lifetime for idle objects.
        /// </summary>
        /// <typeparam name="TConfiguration">The GameObject pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="lifetime">The lifetime in seconds. Use -1 for unlimited.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithIdleLifetime<TConfiguration>(
            this TConfiguration configuration, float lifetime)
            where TConfiguration : IGameObjectPoolConfiguration
        {
            configuration.DefaultIdleLifetime = lifetime;
            return configuration;
        }

        /// <summary>
        /// Sets the interval between tick updates.
        /// </summary>
        /// <typeparam name="TConfiguration">The GameObject pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="interval">The tick interval in seconds.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithTickInterval<TConfiguration>(
            this TConfiguration configuration, float interval)
            where TConfiguration : IGameObjectPoolConfiguration
        {
            configuration.TickInterval = interval;
            return configuration;
        }

        /// <summary>
        /// Configures the pool for unlimited lifetime (no automatic recycling).
        /// </summary>
        /// <typeparam name="TConfiguration">The GameObject pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithUnlimitedLifetime<TConfiguration>(
            this TConfiguration configuration)
            where TConfiguration : IGameObjectPoolConfiguration
        {
            configuration.DefaultActiveLifetime = -1f;
            configuration.DefaultIdleLifetime = -1f;
            return configuration;
        }
    }
}

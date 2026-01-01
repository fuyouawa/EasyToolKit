namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IPoolConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring pool properties.
    /// </summary>
    public static class PoolConfigurationExtensions
    {
        /// <summary>
        /// Sets the initial capacity of the pool.
        /// </summary>
        /// <typeparam name="TConfiguration">The pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="initialCapacity">The initial capacity to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithInitialCapacity<TConfiguration>(
            this TConfiguration configuration, int initialCapacity)
            where TConfiguration : IPoolConfiguration
        {
            configuration.InitialCapacity = initialCapacity;
            return configuration;
        }

        /// <summary>
        /// Sets the maximum capacity of the pool.
        /// </summary>
        /// <typeparam name="TConfiguration">The pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="maxCapacity">The maximum capacity to set. Use -1 for unlimited.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithMaxCapacity<TConfiguration>(
            this TConfiguration configuration, int maxCapacity)
            where TConfiguration : IPoolConfiguration
        {
            configuration.MaxCapacity = maxCapacity;
            return configuration;
        }

        /// <summary>
        /// Configures whether to call <see cref="IPoolItem"/> callbacks.
        /// </summary>
        /// <typeparam name="TConfiguration">The pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="callCallbacks">Whether to call pool item callbacks.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithCallbacks<TConfiguration>(
            this TConfiguration configuration, bool callCallbacks = true)
            where TConfiguration : IPoolConfiguration
        {
            configuration.CallPoolItemCallbacks = callCallbacks;
            return configuration;
        }
    }
}

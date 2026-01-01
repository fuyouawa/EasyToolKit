using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IObjectPoolConfiguration{T}"/> interfaces.
    /// Provides fluent API methods for configuring object pool properties.
    /// </summary>
    public static class ObjectPoolConfigurationExtensions
    {
        /// <summary>
        /// Sets the factory function for creating new instances.
        /// </summary>
        /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
        /// <typeparam name="TConfiguration">The object pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="factory">The factory function to use.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithFactory<T, TConfiguration>(
            this TConfiguration configuration, Func<T> factory)
            where TConfiguration : IObjectPoolConfiguration<T>
            where T : class, new()
        {
            configuration.Factory = factory;
            return configuration;
        }

        /// <summary>
        /// Configures the pool to use default constructor for creating instances.
        /// </summary>
        /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
        /// <typeparam name="TConfiguration">The object pool configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithDefaultConstructor<T, TConfiguration>(
            this TConfiguration configuration)
            where TConfiguration : IObjectPoolConfiguration<T>
            where T : class, new()
        {
            configuration.Factory = null;
            return configuration;
        }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IValueConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring value element properties.
    /// </summary>
    public static class ValueConfigurationExtensions
    {
        /// <summary>
        /// Sets the value type of a value configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The value configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="valueType">The value type to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithValueType<TConfiguration>(this TConfiguration configuration, Type valueType)
            where TConfiguration : IValueConfiguration
        {
            configuration.ValueType = valueType;
            return configuration;
        }

        /// <summary>
        /// Sets the value type of a value configuration using generic type parameter.
        /// </summary>
        /// <typeparam name="T">The value type to set.</typeparam>
        /// <typeparam name="TConfiguration">The value configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithValueType<T, TConfiguration>(this TConfiguration configuration)
            where TConfiguration : IValueConfiguration
        {
            configuration.ValueType = typeof(T);
            return configuration;
        }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base extension methods for <see cref="IElementConfiguration"/> interfaces.
    /// Provides common fluent API methods for configuring element properties.
    /// </summary>
    public static class ElementConfigurationExtensions
    {
        /// <summary>
        /// Sets the name of the element configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="name">The name to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithName<TConfiguration>(this TConfiguration configuration, string name)
            where TConfiguration : IElementConfiguration
        {
            configuration.Name = name;
            return configuration;
        }
    }
}

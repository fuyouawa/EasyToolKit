namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IPropertyConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring property element properties.
    /// </summary>
    public static class PropertyConfigurationExtensions
    {
        /// <summary>
        /// Configures the property to be rendered using Unity's built-in property drawer.
        /// </summary>
        /// <typeparam name="TConfiguration">The property configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="asUnityProperty">Whether to render the property using Unity's built-in property drawer.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration AsUnityProperty<TConfiguration>(this TConfiguration configuration, bool asUnityProperty = true)
            where TConfiguration : IPropertyConfiguration
        {
            configuration.AsUnityProperty = asUnityProperty;
            return configuration;
        }
    }
}

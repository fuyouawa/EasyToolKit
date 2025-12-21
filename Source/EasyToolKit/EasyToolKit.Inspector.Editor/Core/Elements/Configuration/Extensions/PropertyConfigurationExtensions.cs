using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IPropertyConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring property element properties.
    /// </summary>
    public static class PropertyConfigurationExtensions
    {
        /// <summary>
        /// Sets the <see cref="System.Reflection.PropertyInfo"/> for this property configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The property configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="propertyInfo">The property info representing the property.</param>
        /// <param name="autoSetValueType">Whether to automatically set the ValueType based on the property type. Defaults to true.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithPropertyInfo<TConfiguration>(this TConfiguration configuration, PropertyInfo propertyInfo, bool autoSetValueType = true)
            where TConfiguration : IPropertyConfiguration
        {
            configuration.PropertyInfo = propertyInfo;

            if (autoSetValueType)
            {
                configuration.ValueType = propertyInfo.PropertyType;
            }

            return configuration;
        }
    }
}

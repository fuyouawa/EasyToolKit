using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IFieldConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring field element properties.
    /// </summary>
    public static class FieldConfigurationExtensions
    {
        /// <summary>
        /// Sets the <see cref="System.Reflection.FieldInfo"/> for this field configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The field configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="fieldInfo">The field info representing the field.</param>
        /// <param name="autoSetValueType">Whether to automatically set the ValueType based on the field type. Defaults to true.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithFieldInfo<TConfiguration>(this TConfiguration configuration, FieldInfo fieldInfo, bool autoSetValueType = true)
            where TConfiguration : IFieldConfiguration
        {
            configuration.FieldInfo = fieldInfo;

            if (autoSetValueType)
            {
                configuration.ValueType = fieldInfo.FieldType;
            }

            return configuration;
        }

        /// <summary>
        /// Configures the field to be rendered using Unity's built-in property drawer.
        /// </summary>
        /// <typeparam name="TConfiguration">The field configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="asUnityProperty">Whether to render the field using Unity's built-in property drawer.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration AsUnityProperty<TConfiguration>(this TConfiguration configuration, bool asUnityProperty = true)
            where TConfiguration : IFieldConfiguration
        {
            configuration.AsUnityProperty = asUnityProperty;
            return configuration;
        }
    }
}
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IMethodParameterConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring method parameter element properties.
    /// </summary>
    public static class MethodParameterConfigurationExtensions
    {
        /// <summary>
        /// Sets the parameter index for a method parameter configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The method parameter configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="index">The index of the parameter in the method's parameter list.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithParameterIndex<TConfiguration>(this TConfiguration configuration, int index)
            where TConfiguration : IMethodParameterConfiguration
        {
            configuration.ParameterIndex = index;
            return configuration;
        }

        /// <summary>
        /// Sets the <see cref="ParameterInfo"/> for a method parameter configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The method parameter configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="parameterInfo">The <see cref="ParameterInfo"/> that describes the method parameter.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithParameterInfo<TConfiguration>(this TConfiguration configuration, ParameterInfo parameterInfo)
            where TConfiguration : IMethodParameterConfiguration
        {
            configuration.ParameterInfo = parameterInfo;
            return configuration;
        }
    }
}

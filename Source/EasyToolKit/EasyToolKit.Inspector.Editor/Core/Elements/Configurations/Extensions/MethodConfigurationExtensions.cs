using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IMethodConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring method element properties.
    /// </summary>
    public static class MethodConfigurationExtensions
    {
        /// <summary>
        /// Sets the method info for a method configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The method configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="methodInfo">The method info to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithMethodInfo<TConfiguration>(this TConfiguration configuration, MethodInfo methodInfo)
            where TConfiguration : IMethodConfiguration
        {
            configuration.MethodInfo = methodInfo;
            return configuration;
        }

        /// <summary>
        /// Sets the method info for a method configuration using expression tree.
        /// Note: This requires System.Linq.Expressions and additional implementation.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TConfiguration">The method configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithMethodInfo<T, TConfiguration>(this TConfiguration configuration, string methodName)
            where TConfiguration : IMethodConfiguration
        {
            var methodInfo = typeof(T).GetMethod(methodName);
            if (methodInfo == null)
                throw new ArgumentException($"Method '{methodName}' not found on type '{typeof(T).Name}'");

            configuration.MethodInfo = methodInfo;
            return configuration;
        }
    }
}

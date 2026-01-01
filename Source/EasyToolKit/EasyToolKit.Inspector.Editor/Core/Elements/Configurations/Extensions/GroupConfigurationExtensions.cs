using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IGroupConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring group element properties.
    /// </summary>
    public static class GroupConfigurationExtensions
    {
        /// <summary>
        /// Sets the begin and end attribute types for a group configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="beginAttributeType">The attribute type that begins the group.</param>
        /// <param name="endAttributeType">The attribute type that ends the group.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttributes<TConfiguration>(this TConfiguration configuration,
            Type beginAttributeType, Type endAttributeType)
            where TConfiguration : IGroupConfiguration
        {
            configuration.BeginGroupAttributeType = beginAttributeType;
            configuration.EndGroupAttributeType = endAttributeType;
            return configuration;
        }

        /// <summary>
        /// Sets the begin and end attribute types for a group configuration using generic type parameters.
        /// </summary>
        /// <typeparam name="TBeginAttribute">The attribute type that begins the group.</typeparam>
        /// <typeparam name="TEndAttribute">The attribute type that ends the group.</typeparam>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttributes<TBeginAttribute, TEndAttribute, TConfiguration>(this TConfiguration configuration)
            where TBeginAttribute : Attribute
            where TEndAttribute : Attribute
            where TConfiguration : IGroupConfiguration
        {
            configuration.BeginGroupAttributeType = typeof(TBeginAttribute);
            configuration.EndGroupAttributeType = typeof(TEndAttribute);
            return configuration;
        }
    }
}

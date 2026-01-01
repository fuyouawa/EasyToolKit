using System;
using System.Collections;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="ICollectionConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring collection element properties.
    /// </summary>
    public static class CollectionConfigurationExtensions
    {
        /// <summary>
        /// Sets the item type for a collection configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The collection configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="itemType">The type of elements in the collection.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithItemType<TConfiguration>(this TConfiguration configuration, Type itemType)
            where TConfiguration : ICollectionConfiguration
        {
            configuration.ItemType = itemType;
            return configuration;
        }

        /// <summary>
        /// Sets the item type for a collection configuration using a generic type parameter.
        /// </summary>
        /// <typeparam name="TConfiguration">The collection configuration type.</typeparam>
        /// <typeparam name="TItem">The type of elements in the collection.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithItemType<TConfiguration, TItem>(this TConfiguration configuration)
            where TConfiguration : ICollectionConfiguration
        {
            configuration.ItemType = typeof(TItem);
            return configuration;
        }

        /// <summary>
        /// Sets whether the collection is ordered (can be accessed by index).
        /// </summary>
        /// <typeparam name="TConfiguration">The collection configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="isOrdered">True if the collection is ordered; false otherwise.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithIsOrdered<TConfiguration>(this TConfiguration configuration, bool isOrdered)
            where TConfiguration : ICollectionConfiguration
        {
            configuration.IsOrdered = isOrdered;
            return configuration;
        }

        /// <summary>
        /// Marks the collection as ordered (can be accessed by index).
        /// </summary>
        /// <typeparam name="TConfiguration">The collection configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration AsOrdered<TConfiguration>(this TConfiguration configuration)
            where TConfiguration : ICollectionConfiguration
        {
            configuration.IsOrdered = true;
            return configuration;
        }

        /// <summary>
        /// Marks the collection as unordered (cannot be accessed by index).
        /// </summary>
        /// <typeparam name="TConfiguration">The collection configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration AsUnordered<TConfiguration>(this TConfiguration configuration)
            where TConfiguration : ICollectionConfiguration
        {
            configuration.IsOrdered = false;
            return configuration;
        }
    }
}

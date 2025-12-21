namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="ICollectionItemConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring collection item element properties.
    /// </summary>
    public static class CollectionItemConfigurationExtensions
    {
        /// <summary>
        /// Sets the collection item index for a collection item configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The collection item configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="index">The index of the item in the collection.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithItemIndex<TConfiguration>(this TConfiguration configuration, int index)
            where TConfiguration : ICollectionItemConfiguration
        {
            configuration.ItemIndex = index;
            return configuration;
        }
    }
}

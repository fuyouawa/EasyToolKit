using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating collection item element definitions.
    /// Collection items represent individual elements within collections (lists, arrays).
    /// </summary>
    public interface ICollectionItemConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Gets or sets the index of this item within its parent collection.
        /// This determines the position of the item in the collection display.
        /// </summary>
        int ItemIndex { get; set; }

        /// <summary>
        /// Creates a new <see cref="ICollectionItemDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection item definition instance.</returns>
        new ICollectionItemDefinition CreateDefinition();
    }
}
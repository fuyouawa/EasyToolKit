using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating collection element definitions.
    /// Collections represent data structures like arrays, lists, and dictionaries that contain multiple elements.
    /// </summary>
    public interface ICollectionConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Gets or sets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        bool IsOrdered { get; set; }

        /// <summary>
        /// Creates a new <see cref="ICollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection definition instance.</returns>
        new ICollectionDefinition CreateDefinition();
    }
}
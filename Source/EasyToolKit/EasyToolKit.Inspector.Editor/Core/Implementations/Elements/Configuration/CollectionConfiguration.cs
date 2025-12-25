using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base configuration interface for creating collection element definitions.
    /// Collections represent data structures that contain multiple elements.
    /// </summary>
    public class CollectionConfiguration : ValueConfiguration, ICollectionConfiguration
    {
        /// <summary>
        /// Gets or sets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Creates a new <see cref="ICollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection definition instance.</returns>
        public new ICollectionDefinition CreateDefinition()
        {
            if (ValueType == null)
            {
                throw new InvalidOperationException("ValueType cannot be null");
            }

            if (ItemType == null)
            {
                throw new InvalidOperationException("ItemType cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Name cannot be null or whitespace");
            }

            return new CollectionDefinition(ElementFlags.Collection, Name, ValueType, ItemType);
        }
    }
}

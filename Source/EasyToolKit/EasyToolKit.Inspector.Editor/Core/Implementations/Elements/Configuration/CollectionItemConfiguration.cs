using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating collection item element definitions.
    /// Collection items represent individual elements within a collection.
    /// </summary>
    public class CollectionItemConfiguration : ValueConfiguration, ICollectionItemConfiguration
    {
        /// <summary>
        /// Gets or sets the index of this item within its parent collection.
        /// </summary>
        public int ItemIndex { get; set; }

        /// <summary>
        /// Creates a new <see cref="ICollectionItemDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new collection item definition instance.</returns>
        public ICollectionItemDefinition CreateDefinition()
        {
            if (ValueType == null)
            {
                throw new InvalidOperationException("ValueType cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Name cannot be null or whitespace");
            }

            return new CollectionItemDefinition(ElementFlags.CollectionItem | ElementFlags.Value, Name, ValueType, ItemIndex);
        }
    }
}

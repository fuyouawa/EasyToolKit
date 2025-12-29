using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Collection item definition implementation handling individual elements in collections.
    /// Similar to dynamically created custom values, representing individual element items in collections.
    /// </summary>
    public sealed class CollectionItemDefinition : ValueDefinition, ICollectionItemDefinition
    {
        /// <summary>
        /// Gets or sets the index of this item within its parent collection.
        /// </summary>
        public int ItemIndex { get; set; }
    }
}

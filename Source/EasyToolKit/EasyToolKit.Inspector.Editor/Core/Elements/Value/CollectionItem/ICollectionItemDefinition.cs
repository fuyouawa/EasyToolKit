using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection item definition in the inspector.
    /// </summary>
    public interface ICollectionItemDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the index of this item within its parent collection.
        /// </summary>
        int CollectionItemIndex { get; }
    }
}

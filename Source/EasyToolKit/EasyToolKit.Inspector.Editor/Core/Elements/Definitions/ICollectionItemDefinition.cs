using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Collection item definition interface handling the abstract concept of elements in collections.
    /// Similar to dynamically created custom values, representing individual element items in collections.
    /// </summary>
    public interface ICollectionItemDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the index of this item within its parent collection.
        /// </summary>
        int ItemIndex { get; }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Collection definition interface for describing collection data structures like arrays, lists, and dictionaries.
    /// Inherits from <see cref="IValueDefinition"/> and extends it with collection-specific metadata.
    /// </summary>
    public interface ICollectionDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        Type ItemType { get; }
    }
}
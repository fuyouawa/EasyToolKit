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

        /// <summary>
        /// Gets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        bool IsOrdered { get; }
    }
}
using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Collection definition implementation for describing collection data structures.
    /// Inherits from <see cref="ValueDefinition"/> and extends it with collection-specific metadata.
    /// </summary>
    public class CollectionDefinition : ValueDefinition, ICollectionDefinition
    {
        /// <summary>
        /// Gets or sets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        public bool IsOrdered { get; set; }
    }
}

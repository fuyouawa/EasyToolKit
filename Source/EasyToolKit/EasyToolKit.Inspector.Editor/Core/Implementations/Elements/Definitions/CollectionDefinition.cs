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
        /// Initializes a new instance of the <see cref="CollectionDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="valueType">The type of the collection.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered (can be accessed by index).</param>
        public CollectionDefinition(ElementRoles roles, string name, Type valueType, Type itemType, bool isOrdered)
            : base(roles, name, valueType)
        {
            ItemType = itemType;
            IsOrdered = isOrdered;
        }

        /// <summary>
        /// Gets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        /// Gets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        public bool IsOrdered { get; }
    }
}

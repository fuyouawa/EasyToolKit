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
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="valueType">The type of the collection.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        public CollectionDefinition(ElementFlags flags, string name, Type valueType, Type itemType)
            : base(flags, name, valueType)
        {
            ItemType = itemType;
        }

        /// <summary>
        /// Gets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; }
    }
}

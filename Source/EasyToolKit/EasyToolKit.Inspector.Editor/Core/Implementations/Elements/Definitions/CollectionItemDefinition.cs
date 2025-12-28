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
        /// Initializes a new instance of the <see cref="CollectionItemDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="itemIndex">The index of this item within its parent collection.</param>
        public CollectionItemDefinition(ElementRoles roles, string name, Type valueType, int itemIndex)
            : base(roles, name, valueType)
        {
            ItemIndex = itemIndex;
        }

        /// <summary>
        /// Gets the index of this item within its parent collection.
        /// </summary>
        public int ItemIndex { get; }
    }
}

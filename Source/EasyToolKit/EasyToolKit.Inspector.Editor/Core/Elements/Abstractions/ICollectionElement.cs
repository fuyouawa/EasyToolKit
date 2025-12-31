namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Collection element interface for representing collection data structures like arrays, lists, and dictionaries.
    /// Inherits from <see cref="IValueElement"/> to provide value-based functionality while specializing for collection types.
    /// </summary>
    public interface ICollectionElement : IValueElement
    {
        /// <summary>
        /// Gets the collection definition that describes this collection element.
        /// </summary>
        new ICollectionDefinition Definition { get; }

        new IReadOnlyElementList<ICollectionItemElement> LogicalChildren { get; }

        new IElementList<IElement> Children { get; }

        /// <summary>
        /// Gets the base value entry that is built directly from <see cref="IValueDefinition.ValueType"/>.
        /// This represents the declared type of the value.
        /// </summary>
        new ICollectionEntry BaseValueEntry { get; }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// This is built based on the runtime type of the value in <see cref="BaseValueEntry"/>.
        /// </summary>
        /// <remarks>
        /// <para>When the runtime type equals the declared type, this is the same as <see cref="BaseValueEntry"/>.</para>
        /// <para>When the runtime type is a derived type, this is a type wrapper around <see cref="BaseValueEntry"/>.</para>
        /// </remarks>
        new ICollectionEntry ValueEntry { get; }
    }
}

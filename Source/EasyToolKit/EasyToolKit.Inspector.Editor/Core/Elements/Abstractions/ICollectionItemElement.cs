namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Collection item element interface representing individual elements in collections (arrays, lists, etc.).
    /// As an abstract collection element concept, supports dynamic creation and management of individual data items in collections.
    /// </summary>
    public interface ICollectionItemElement : IValueElement
    {
        /// <summary>
        /// Gets the collection item definition that describes this collection item.
        /// </summary>
        new ICollectionItemDefinition Definition { get; }

        new ICollectionElement LogicalParent { get; }
    }
}

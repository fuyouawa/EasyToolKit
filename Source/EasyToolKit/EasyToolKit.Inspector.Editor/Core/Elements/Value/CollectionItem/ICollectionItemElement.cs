namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection item element in the inspector tree.
    /// </summary>
    public interface ICollectionItemElement : IValueElement
    {
        /// <summary>
        /// Gets the collection item definition that describes this collection item.
        /// </summary>
        new ICollectionItemDefinition Definition { get; }
    }
}

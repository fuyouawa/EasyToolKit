namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property collection element interface representing collection properties on an object.
    /// Combines the collection functionality of <see cref="ICollectionElement"/> with the property-specific features of <see cref="IPropertyElement"/>.
    /// </summary>
    public interface IPropertyCollectionElement : ICollectionElement, IPropertyElement
    {
        /// <summary>
        /// Gets the property collection definition that describes this property collection.
        /// </summary>
        new IPropertyCollectionDefinition Definition { get; }
    }
}
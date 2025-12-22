namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Field collection element interface representing collection fields on an object.
    /// Combines the collection functionality of <see cref="ICollectionElement"/> with the field-specific features of <see cref="IFieldElement"/>.
    /// </summary>
    public interface IFieldCollectionElement : ICollectionElement, IFieldElement
    {
        /// <summary>
        /// Gets the field collection definition that describes this field collection.
        /// </summary>
        new IFieldCollectionDefinition Definition { get; }
    }
}
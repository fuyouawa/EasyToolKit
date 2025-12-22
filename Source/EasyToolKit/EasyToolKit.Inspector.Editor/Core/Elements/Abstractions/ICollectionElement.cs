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
    }
}
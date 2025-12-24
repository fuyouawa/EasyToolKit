namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for ordered collection entry wrappers.
    /// </summary>
    public interface IOrderedCollectionEntryWrapper : ICollectionEntryWrapper, IOrderedCollectionEntry
    {
    }

    /// <summary>
    /// Type-safe wrapper for an ordered collection entry that exposes a more derived collection type.
    /// </summary>
    /// <typeparam name="TCollection">The derived ordered collection type exposed by this wrapper.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <typeparam name="TBaseCollection">The base ordered collection type stored in the underlying ordered collection entry.</typeparam>
    /// <typeparam name="TBaseItem">The base item type stored in the underlying collection entry.</typeparam>
    public interface IOrderedCollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem> :
        ICollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>,
        IOrderedCollectionEntry<TCollection, TItem>,
        IOrderedCollectionEntryWrapper
        where TBaseCollection : notnull
        where TCollection : TBaseCollection
    {
        /// <summary>
        /// Gets the underlying collection entry.
        /// </summary>
        new IOrderedCollectionEntry<TBaseCollection, TBaseItem> BaseValueEntry { get; }
    }
}

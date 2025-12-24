namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for collection entry wrappers.
    /// </summary>
    public interface ICollectionEntryWrapper : IValueEntryWrapper, ICollectionEntry
    {
    }

    /// <summary>
    /// Type-safe wrapper for a collection entry that exposes a more derived collection type.
    /// </summary>
    /// <typeparam name="TCollection">The derived collection type exposed by this wrapper.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <typeparam name="TBaseCollection">The base collection type stored in the underlying collection entry.</typeparam>
    /// <typeparam name="TBaseItem">The base item type stored in the underlying collection entry.</typeparam>
    public interface ICollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem> :
        IValueEntryWrapper<TCollection, TBaseCollection>,
        ICollectionEntry<TCollection, TItem>,
        ICollectionEntryWrapper
        where TBaseCollection : notnull
        where TCollection : TBaseCollection
    {
        /// <summary>
        /// Gets the underlying collection entry.
        /// </summary>
        new ICollectionEntry<TBaseCollection, TBaseItem> BaseValueEntry { get; }
    }
}

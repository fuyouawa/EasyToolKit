namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines a collection value entry with change tracking and notifications.
    /// </summary>
    public interface ICollectionEntry : IValueEntry, ICollectionAccessor, ICollectionChangeHandler
    {
    }

    public interface IOrderedCollectionEntry : ICollectionEntry, IOrderedCollectionAccessor
    {
    }

    public interface ICollectionEntry<TCollection, TItem> :
        ICollectionEntry,
        IValueEntry<TCollection>,
        ICollectionAccessor<TCollection, TItem>
    {
    }

    public interface IOrderedCollectionEntry<TCollection, TItem> :
        IOrderedCollectionEntry,
        ICollectionEntry<TCollection, TItem>,
        IOrderedCollectionAccessor<TCollection, TItem>
    {
    }
}

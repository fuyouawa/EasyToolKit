namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionEntry : IValueEntry, ICollectionAccessor
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

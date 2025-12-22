using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionAccessor : IValueAccessor
    {
        Type ItemType { get; }

        object AddWeakItem(int targetIndex, object value);
        void RemoveWeakItem(int targetIndex, object value);
    }

    public interface ICollectionAccessor<TCollection, TItem> : ICollectionAccessor, IValueAccessor<TCollection>
    {
        void AddItem(int targetIndex, TItem value);
        void RemoveItem(int targetIndex, TItem value);
    }

    public interface IOrderedCollectionAccessor : ICollectionAccessor
    {
        void InsertWeakItem(int targetIndex, int itemIndex, object value);
        void RemoveItem(int targetIndex, int itemIndex);
    }

    public interface IOrderedCollectionAccessor<TCollection, TItem> : ICollectionAccessor<TCollection, TItem>, IOrderedCollectionAccessor
    {
        void InsertItem(int targetIndex, int itemIndex, TItem value);
    }
}

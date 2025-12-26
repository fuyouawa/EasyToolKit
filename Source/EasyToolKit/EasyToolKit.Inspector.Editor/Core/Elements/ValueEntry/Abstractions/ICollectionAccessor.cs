using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionAccessor : IValueAccessor
    {
        Type ItemType { get; }
        Type RuntimeItemType { get; }

        int GetItemCount(int targetIndex);
        void AddWeakItem(int targetIndex, object value);
        void RemoveWeakItem(int targetIndex, object value);
    }

    public interface ICollectionAccessor<TCollection, TItem> : ICollectionAccessor, IValueAccessor<TCollection>
    {
        void AddItem(int targetIndex, TItem value);
        void RemoveItem(int targetIndex, TItem value);
    }

    public interface IOrderedCollectionAccessor : ICollectionAccessor
    {
        object GetWeakItemAt(int targetIndex, int itemIndex);
        void InsertWeakItemAt(int targetIndex, int itemIndex, object value);
        void RemoveItemAt(int targetIndex, int itemIndex);
    }

    public interface IOrderedCollectionAccessor<TCollection, TItem> : ICollectionAccessor<TCollection, TItem>, IOrderedCollectionAccessor
    {
        TItem GetItemAt(int targetIndex, int itemIndex);
        void InsertItemAt(int targetIndex, int itemIndex, TItem value);
    }
}

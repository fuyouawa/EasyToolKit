using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IOrderedCollectionOperation : ICollectionOperation
    {
        void InsertWeakItem(ref object collection, int index, object value);

        void RemoveWeakItem(ref object collection, int index);
    }

    public interface IOrderedCollectionOperation<TCollection, TItem> : IOrderedCollectionOperation, ICollectionOperation<TCollection, TItem>
    {
        void InsertItem(ref TCollection collection, int index, TItem value);

        void RemoveItem(ref TCollection collection, int index);
    }
}

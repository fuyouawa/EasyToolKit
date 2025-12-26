using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IOrderedCollectionOperation : ICollectionOperation
    {
        object GetWeakItemAt(ref object collection, int index);

        void InsertWeakItemAt(ref object collection, int index, object value);

        void RemoveWeakItemAt(ref object collection, int index);
    }

    public interface IOrderedCollectionOperation<TCollection, TItem> : IOrderedCollectionOperation, ICollectionOperation<TCollection, TItem>
    {
        TItem GetItemAt(ref TCollection collection, int index);

        void InsertItemAt(ref TCollection collection, int index, TItem value);

        void RemoveItemAt(ref TCollection collection, int index);
    }
}

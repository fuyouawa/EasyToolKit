using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionOperation : IValueOperation
    {
        Type ItemType { get; }
        Type GetItemRuntimeType(ref object collection);

        void AddWeakItem(ref object collection, object value);

        void RemoveWeakItem(ref object collection, object value);
    }

    public interface ICollectionOperation<TCollection, TItem> : ICollectionOperation, IValueOperation<TCollection>
    {
        void AddItem(ref TCollection collection, TItem value);
        void RemoveItem(ref TCollection collection, TItem value);
    }
}

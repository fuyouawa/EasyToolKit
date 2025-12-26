using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class OrderedCollectionOperationBase<TCollection, TValue> : CollectionOperationBase<TCollection, TValue>, IOrderedCollectionOperation<TCollection, TValue>
    {
        protected OrderedCollectionOperationBase(Type ownerType) : base(ownerType)
        {
        }

        public abstract TValue GetItemAt(ref TCollection collection, int index);

        public abstract void InsertItemAt(ref TCollection collection, int index, TValue value);

        public abstract void RemoveItemAt(ref TCollection collection, int index);

        public object GetWeakItemAt(ref object collection, int index)
        {
            var castCollection = (TCollection)collection;
            return GetItemAt(ref castCollection, index);
        }

        public virtual void InsertWeakItemAt(ref object collection, int index, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TValue)value;
            InsertItemAt(ref castCollection, index, castValue);
            collection = castCollection;
        }

        public virtual void RemoveWeakItemAt(ref object collection, int index)
        {
            var castCollection = (TCollection)collection;
            RemoveItemAt(ref castCollection, index);
            collection = castCollection;
        }
    }
}

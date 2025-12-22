using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class OrderedCollectionOperationBase<TCollection, TValue> : CollectionOperationBase<TCollection, TValue>, IOrderedCollectionOperation<TCollection, TValue>
    {
        protected OrderedCollectionOperationBase(Type ownerType) : base(ownerType)
        {
        }

        public abstract void InsertItem(ref TCollection collection, int index, TValue value);

        public abstract void RemoveItem(ref TCollection collection, int index);

        public virtual void InsertWeakItem(ref object collection, int index, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TValue)value;
            InsertItem(ref castCollection, index, castValue);
            collection = castCollection;
        }

        public virtual void RemoveWeakItem(ref object collection, int index)
        {
            var castCollection = (TCollection)collection;
            RemoveItem(ref castCollection, index);
            collection = castCollection;
        }
    }
}

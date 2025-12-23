using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class CollectionOperationBase : ICollectionOperation
    {
        protected CollectionOperationBase(Type ownerType)
        {
            OwnerType = ownerType;
        }

        public virtual bool IsReadOnly => false;

        public virtual Type OwnerType { get; }

        public abstract Type ValueType { get; }

        public abstract Type ItemType { get; }

        public virtual object GetWeakValue(ref object owner)
        {
            throw new NotSupportedException("Getting values is not supported for collection operations.");
        }

        public virtual void SetWeakValue(ref object owner, object value)
        {
            throw new NotSupportedException("Setting values is not supported for collection operations.");
        }

        public abstract void AddWeakItem(ref object collection, object value);

        public abstract void RemoveWeakItem(ref object collection, object value);
    }

    public abstract class CollectionOperationBase<TCollection, TItem> : CollectionOperationBase, ICollectionOperation<TCollection, TItem>
    {
        protected CollectionOperationBase(Type ownerType) : base(ownerType)
        {
        }

        public override Type ValueType => typeof(TCollection);

        public override Type ItemType => typeof(TItem);

        public virtual TCollection GetValue(ref object owner)
        {
            throw new NotSupportedException("Getting values is not supported for collection operations.");
        }

        public virtual void SetValue(ref object owner, TCollection value)
        {
            throw new NotSupportedException("Setting values is not supported for collection operations.");
        }

        public abstract void AddItem(ref TCollection collection, TItem value);

        public abstract void RemoveItem(ref TCollection collection, TItem value);

        public override object GetWeakValue(ref object owner)
        {
            return GetValue(ref owner);
        }

        public override void SetWeakValue(ref object owner, object value)
        {
            var castValue = (TCollection)value;
            SetValue(ref owner, castValue);
        }

        public override void AddWeakItem(ref object collection, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TItem)value;
            AddItem(ref castCollection, castValue);
            collection = castCollection;
        }

        public override void RemoveWeakItem(ref object collection, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TItem)value;
            RemoveItem(ref castCollection, castValue);
            collection = castCollection;
        }
    }
}

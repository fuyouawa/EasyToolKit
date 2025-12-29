using System;
using EasyToolKit.Core;

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

        public abstract Type CollectionType { get; }
        public abstract Type ItemType { get; }

        public abstract Type GetItemRuntimeType(ref object collection);

        public virtual Type GetCollectionRuntimeType(ref object owner)
        {
            throw new NotSupportedException($"Getting runtime type for '{owner}' is not supported for collection operations.");
        }

        public virtual object GetWeakValue(ref object owner)
        {
            throw new NotSupportedException($"Getting values for '{owner}' is not supported for collection operations.");
        }

        public virtual void SetWeakValue(ref object owner, object value)
        {
            throw new NotSupportedException($"Setting values for '{owner}' is not supported for collection operations.");
        }

        public abstract int GetWeakItemCount(ref object collection);
        public abstract void AddWeakItem(ref object collection, object value);

        public abstract void RemoveWeakItem(ref object collection, object value);

        Type IValueOperation.ValueType => CollectionType;

        Type IValueOperation.GetValueRuntimeType(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetCollectionRuntimeType(ref owner);
        }

        int ICollectionOperation.GetWeakItemCount(ref object collection)
        {
            var c = collection;
            Assert.IsTrue(collection.GetType().IsInheritsFrom(CollectionType),
                () => $"Collection type mismatch. Expected: {CollectionType}, Actual: {c.GetType()}");
            return GetWeakItemCount(ref collection);
        }

        object IValueOperation.GetWeakValue(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetWeakValue(ref owner);
        }

        void IValueOperation.SetWeakValue(ref object owner, object value)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");

            Assert.IsTrue(value == null || value.GetType().IsInheritsFrom(CollectionType),
                () => $"Collection type mismatch. Expected: {CollectionType}, Actual: {value?.GetType()}");
            SetWeakValue(ref owner, value);
        }

        Type ICollectionOperation.GetItemRuntimeType(ref object collection)
        {
            var c = collection;
            Assert.IsTrue(collection.GetType().IsInheritsFrom(CollectionType),
                () => $"Collection type mismatch. Expected: {CollectionType}, Actual: {c.GetType()}");
            return GetItemRuntimeType(ref collection);
        }

        void ICollectionOperation.AddWeakItem(ref object collection, object value)
        {
            var c = collection;
            Assert.IsTrue(collection.GetType().IsInheritsFrom(CollectionType),
                () => $"Collection type mismatch. Expected: {CollectionType}, Actual: {c.GetType()}");

            Assert.IsTrue(value == null || value.GetType().IsInheritsFrom(ItemType),
                () => $"Item type mismatch. Expected: {ItemType}, Actual: {value?.GetType()}");
            AddWeakItem(ref collection, value);
        }

        void ICollectionOperation.RemoveWeakItem(ref object collection, object value)
        {
            var c = collection;
            Assert.IsTrue(collection.GetType().IsInheritsFrom(CollectionType),
                () => $"Collection type mismatch. Expected: {CollectionType}, Actual: {c.GetType()}");

            Assert.IsTrue(value == null || value.GetType().IsInheritsFrom(ItemType),
                () => $"Item type mismatch. Expected: {ItemType}, Actual: {value?.GetType()}");
            RemoveWeakItem(ref collection, value);
        }
    }

    public abstract class CollectionOperationBase<TCollection, TItem> : CollectionOperationBase, ICollectionOperation<TCollection, TItem>
    {
        protected CollectionOperationBase(Type ownerType) : base(ownerType)
        {
        }

        public override Type CollectionType => typeof(TCollection);

        public override Type ItemType => typeof(TItem);

        public virtual TCollection GetValue(ref object owner)
        {
            throw new NotSupportedException("Getting values is not supported for collection operations.");
        }

        public virtual void SetValue(ref object owner, TCollection value)
        {
            throw new NotSupportedException("Setting values is not supported for collection operations.");
        }

        public abstract Type GetItemRuntimeType(ref TCollection collection);

        public abstract int GetItemCount(ref TCollection collection);
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

        public override int GetWeakItemCount(ref object collection)
        {
            var castCollection = (TCollection)collection;
            return GetItemCount(ref castCollection);
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

        public override Type GetItemRuntimeType(ref object collection)
        {
            var castCollection = (TCollection)collection;
            return GetItemRuntimeType(ref castCollection);
        }

        TCollection IValueOperation<TCollection>.GetValue(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetValue(ref owner);
        }

        void IValueOperation<TCollection>.SetValue(ref object owner, TCollection value)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            SetValue(ref owner, value);
        }
    }
}

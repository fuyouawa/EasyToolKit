using System;

namespace EasyToolKit.Inspector.Editor
{
    public class CollectionOperationWrapper : ICollectionOperation
    {
        private readonly ICollectionOperation _collectionOperation;
        private readonly IValueOperation _auxiliaryOperation;

        public CollectionOperationWrapper(IValueOperation auxiliaryOperation, ICollectionOperation collectionOperation)
        {
            if (auxiliaryOperation.IsReadOnly != collectionOperation.IsReadOnly ||
                auxiliaryOperation.OwnerType != collectionOperation.OwnerType ||
                auxiliaryOperation.ValueType != collectionOperation.ValueType)
            {
                throw new ArgumentException("Auxiliary operation and collection operation do not match", nameof(auxiliaryOperation));
            }

            _auxiliaryOperation = auxiliaryOperation;
            _collectionOperation = collectionOperation;
        }

        public bool IsReadOnly => _collectionOperation.IsReadOnly;
        public Type OwnerType => _collectionOperation.OwnerType;
        public Type ValueType => _collectionOperation.ValueType;

        public Type ItemType => _collectionOperation.ItemType;

        public Type GetValueRuntimeType(ref object owner)
        {
            return _auxiliaryOperation.GetValueRuntimeType(ref owner);
        }

        public object GetWeakValue(ref object owner)
        {
            return _auxiliaryOperation.GetWeakValue(ref owner);
        }

        public void SetWeakValue(ref object owner, object value)
        {
            _auxiliaryOperation.SetWeakValue(ref owner, value);
        }

        public Type GetItemRuntimeType(ref object collection)
        {
            return _collectionOperation.GetItemRuntimeType(ref collection);
        }

        public int GetWeakItemCount(ref object collection)
        {
            return _collectionOperation.GetWeakItemCount(ref collection);
        }

        public void AddWeakItem(ref object collection, object value)
        {
            _collectionOperation.AddWeakItem(ref collection, value);
        }

        public void RemoveWeakItem(ref object collection, object value)
        {
            _collectionOperation.RemoveWeakItem(ref collection, value);
        }
    }

    public class CollectionOperationWrapper<TCollection, TItem> : CollectionOperationWrapper, ICollectionOperation<TCollection, TItem>
    {
        private readonly IValueOperation<TCollection> _auxiliaryOperation;
        private readonly ICollectionOperation<TCollection, TItem> _collectionOperation;

        public CollectionOperationWrapper(
            IValueOperation<TCollection> auxiliaryOperation,
            ICollectionOperation<TCollection, TItem> collectionOperation)
            : base(auxiliaryOperation, collectionOperation)
        {
            _auxiliaryOperation = auxiliaryOperation;
            _collectionOperation = collectionOperation;
        }

        public TCollection GetValue(ref object owner)
        {
            return _auxiliaryOperation.GetValue(ref owner);
        }

        public void SetValue(ref object owner, TCollection value)
        {
            _auxiliaryOperation.SetValue(ref owner, value);
        }

        public int GetItemCount(ref TCollection collection)
        {
            return _collectionOperation.GetItemCount(ref collection);
        }

        public void AddItem(ref TCollection collection, TItem value)
        {
            _collectionOperation.AddItem(ref collection, value);
        }

        public void RemoveItem(ref TCollection collection, TItem value)
        {
            _collectionOperation.RemoveItem(ref collection, value);
        }
    }
}

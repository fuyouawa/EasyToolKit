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

        public Type CollectionType => _collectionOperation.CollectionType;
        public Type ElementType => _collectionOperation.ElementType;

        public object GetWeakValue(ref object owner)
        {
            return _auxiliaryOperation.GetWeakValue(ref owner);
        }

        public void SetWeakValue(ref object owner, object value)
        {
            _auxiliaryOperation.SetWeakValue(ref owner, value);
        }

        public void AddWeakElement(ref object collection, object value)
        {
            _collectionOperation.AddWeakElement(ref collection, value);
        }

        public void RemoveWeakElement(ref object collection, object value)
        {
            _collectionOperation.RemoveWeakElement(ref collection, value);
        }
    }

    public class CollectionOperationWrapper<TCollection, TElement> : CollectionOperationWrapper, ICollectionOperation<TCollection, TElement>
    {
        private readonly IValueOperation<TCollection> _auxiliaryOperation;
        private readonly ICollectionOperation<TCollection, TElement> _collectionOperation;

        public CollectionOperationWrapper(
            IValueOperation<TCollection> auxiliaryOperation,
            ICollectionOperation<TCollection, TElement> collectionOperation)
            : base(auxiliaryOperation, collectionOperation)
        {
        }

        public TCollection GetValue(ref object owner)
        {
            return _auxiliaryOperation.GetValue(ref owner);
        }

        public void SetValue(ref object owner, TCollection value)
        {
            _auxiliaryOperation.SetValue(ref owner, value);
        }

        public void AddElement(ref TCollection collection, TElement value)
        {
            _collectionOperation.AddElement(ref collection, value);
        }

        public void RemoveElement(ref TCollection collection, TElement value)
        {
            _collectionOperation.RemoveElement(ref collection, value);
        }
    }
}

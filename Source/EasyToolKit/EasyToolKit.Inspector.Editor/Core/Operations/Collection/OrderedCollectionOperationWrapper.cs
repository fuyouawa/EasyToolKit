namespace EasyToolKit.Inspector.Editor
{
    public class OrderedCollectionOperationWrapper : CollectionOperationWrapper, IOrderedCollectionOperation
    {
        private readonly IOrderedCollectionOperation _orderedCollectionOperation;

        public OrderedCollectionOperationWrapper(
            IValueOperation auxiliaryOperation,
            IOrderedCollectionOperation collectionOperation)
            : base(auxiliaryOperation, collectionOperation)
        {
            _orderedCollectionOperation = collectionOperation;
        }

        public void InsertWeakElementAt(ref object collection, int index, object value)
        {
            _orderedCollectionOperation.InsertWeakElementAt(ref collection, index, value);
        }

        public void RemoveWeakElementAt(ref object collection, int index)
        {
            _orderedCollectionOperation.RemoveWeakElementAt(ref collection, index);
        }
    }

    public class OrderedCollectionOperationWrapper<TCollection, TElement> : CollectionOperationWrapper<TCollection, TElement>,
        IOrderedCollectionOperation<TCollection, TElement>
    {
        private readonly IOrderedCollectionOperation<TCollection, TElement> _orderedCollectionOperation;

        public OrderedCollectionOperationWrapper(
            IValueOperation<TCollection> auxiliaryOperation,
            IOrderedCollectionOperation<TCollection, TElement> collectionOperation)
            : base(auxiliaryOperation, collectionOperation)
        {
            _orderedCollectionOperation = collectionOperation;
        }

        public void InsertWeakElementAt(ref object collection, int index, object value)
        {
            _orderedCollectionOperation.InsertWeakElementAt(ref collection, index, value);
        }

        public void RemoveWeakElementAt(ref object collection, int index)
        {
            _orderedCollectionOperation.RemoveWeakElementAt(ref collection, index);
        }

        public void InsertElementAt(ref TCollection collection, int index, TElement value)
        {
            _orderedCollectionOperation.InsertElementAt(ref collection, index, value);
        }

        public void RemoveElementAt(ref TCollection collection, int index)
        {
            _orderedCollectionOperation.RemoveElementAt(ref collection, index);
        }
    }
}

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

        public void InsertWeakItem(ref object collection, int index, object value)
        {
            _orderedCollectionOperation.InsertWeakItem(ref collection, index, value);
        }

        public void RemoveWeakItem(ref object collection, int index)
        {
            _orderedCollectionOperation.RemoveWeakItem(ref collection, index);
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

        public void InsertWeakItem(ref object collection, int index, object value)
        {
            _orderedCollectionOperation.InsertWeakItem(ref collection, index, value);
        }

        public void RemoveWeakItem(ref object collection, int index)
        {
            _orderedCollectionOperation.RemoveWeakItem(ref collection, index);
        }

        public void InsertItem(ref TCollection collection, int index, TElement value)
        {
            _orderedCollectionOperation.InsertItem(ref collection, index, value);
        }

        public void RemoveItem(ref TCollection collection, int index)
        {
            _orderedCollectionOperation.RemoveItem(ref collection, index);
        }
    }
}

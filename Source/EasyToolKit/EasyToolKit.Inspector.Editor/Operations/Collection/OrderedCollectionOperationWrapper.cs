namespace EasyToolKit.Inspector.Editor
{
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

        public object GetWeakItemAt(ref object collection, int index)
        {
            return _orderedCollectionOperation.GetWeakItemAt(ref collection, index);
        }

        public void InsertWeakItemAt(ref object collection, int index, object value)
        {
            _orderedCollectionOperation.InsertWeakItemAt(ref collection, index, value);
        }

        public void RemoveWeakItemAt(ref object collection, int index)
        {
            _orderedCollectionOperation.RemoveWeakItemAt(ref collection, index);
        }

        public TElement GetItemAt(ref TCollection collection, int index)
        {
            return _orderedCollectionOperation.GetItemAt(ref collection, index);
        }

        public void InsertItemAt(ref TCollection collection, int index, TElement value)
        {
            _orderedCollectionOperation.InsertItemAt(ref collection, index, value);
        }

        public void RemoveItemAt(ref TCollection collection, int index)
        {
            _orderedCollectionOperation.RemoveItemAt(ref collection, index);
        }
    }
}

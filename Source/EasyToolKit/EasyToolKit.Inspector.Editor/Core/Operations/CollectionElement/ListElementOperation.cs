using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class ListElementOperation<TCollection, TValue> : CollectionElementOperationBase<TCollection, TValue>
        where TCollection : IList<TValue>
    {
        public ListElementOperation(int elementIndex) : base(elementIndex)
        {
        }

        public override TValue GetElementValue(ref TCollection collection)
        {
            return collection[ElementIndex];
        }

        public override void SetElementValue(ref TCollection collection, TValue value)
        {
            collection[ElementIndex] = value;
        }
    }
}

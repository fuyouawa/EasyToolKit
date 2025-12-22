using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class ReadOnlyListElementOperation<TCollection, TValue> : CollectionElementOperationBase<TCollection, TValue>
        where TCollection : IReadOnlyList<TValue>
    {
        public ReadOnlyListElementOperation(int elementIndex) : base(elementIndex)
        {
        }

        public override TValue GetElementValue(ref TCollection collection)
        {
            return collection[ElementIndex];
        }

        public override void SetElementValue(ref TCollection collection, TValue value)
        {
            throw new InvalidOperationException("ReadOnlyList cannot set element value");
        }
    }
}

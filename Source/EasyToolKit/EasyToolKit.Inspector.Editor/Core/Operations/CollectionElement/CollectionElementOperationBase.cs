using System;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class CollectionElementOperationBase<TCollection, TValue> : ValueOperationBase<TValue>, ICollectionElementOperation<TValue>
    {
        private readonly int _elementIndex;

        protected CollectionElementOperationBase(int elementIndex) : base(typeof(TCollection))
        {
            _elementIndex = elementIndex;
        }

        public int ElementIndex => _elementIndex;

        public override TValue GetValue(ref object owner)
        {
            var castedOwner = (TCollection)owner;
            return GetElementValue(ref castedOwner);
        }

        public override void SetValue(ref object owner, TValue value)
        {
            var castedOwner = (TCollection)owner;
            SetElementValue(ref castedOwner, value);
            owner = castedOwner;
        }

        public abstract TValue GetElementValue(ref TCollection collection);
        public abstract void SetElementValue(ref TCollection collection, TValue value);
    }
}

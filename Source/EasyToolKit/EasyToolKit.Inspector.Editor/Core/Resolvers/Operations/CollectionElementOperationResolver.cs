using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class CollectionElementOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolve(InspectorProperty property)
        {
            return property.Info.IsCollectionElement;
        }

        protected override void Initialize()
        {
            var elementType = Property.BaseValueEntry.ValueType;
            var collectionType = Property.Parent.ValueEntry.ValueType;

            Type operationType;
            if (collectionType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                operationType = typeof(ListElementOperation<,>).MakeGenericType(collectionType, elementType);
            }
            else if (collectionType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                operationType = typeof(ReadOnlyListElementOperation<,>).MakeGenericType(collectionType, elementType);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported collection type '{elementType}'.");
            }

            _operation = operationType.CreateInstance<IPropertyOperation>(Property.Info.CollectionElementIndex);
        }

        public override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}

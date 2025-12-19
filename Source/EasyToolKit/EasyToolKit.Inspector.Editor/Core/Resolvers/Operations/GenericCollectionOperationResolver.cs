using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericCollectionOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _auxiliaryOperation;
        private ICollectionOperation _collectionOperation;

        private ICollectionOperation _operationWrapper;

        protected override bool CanResolve(InspectorProperty property)
        {
            var collectionType = Property.ValueEntry.ValueType;
            return collectionType.IsInheritsFrom<IEnumerable>();
        }

        protected override void Initialize()
        {
            var auxiliaryResolver = new GenericPropertyOperationResolver();
            ((IInspectorElement)auxiliaryResolver).Property = Property;
            ((IInspectorElement)auxiliaryResolver).Initialize();
            _auxiliaryOperation = auxiliaryResolver.GetOperation();

            var ownerType = Property.Parent.ValueEntry.ValueType;
            var collectionType = Property.ValueEntry.ValueType;
            var elementType = collectionType.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];
            bool isOrdered = false;

            if (collectionType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, elementType);
                _collectionOperation = operationType.CreateInstance<ICollectionOperation>(ownerType);
                isOrdered = true;
            }
            else if (collectionType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, elementType);
                _collectionOperation = operationType.CreateInstance<ICollectionOperation>(ownerType);
                isOrdered = true;
            }
            else
            {
                throw new NotSupportedException($"Collection type {collectionType} is not supported.");
            }

            if (isOrdered)
            {
                var operationWrapperType = typeof(OrderedCollectionOperationWrapper<,>).MakeGenericType(collectionType, elementType);
                _operationWrapper = operationWrapperType.CreateInstance<ICollectionOperation>(_auxiliaryOperation, _collectionOperation);
            }
            else
            {
                var operationWrapperType = typeof(CollectionOperationWrapper<,>).MakeGenericType(collectionType, elementType);
                _operationWrapper = operationWrapperType.CreateInstance<ICollectionOperation>(_auxiliaryOperation, _collectionOperation);
            }
        }

        public override IPropertyOperation GetOperation()
        {
            return _operationWrapper;
        }
    }
}

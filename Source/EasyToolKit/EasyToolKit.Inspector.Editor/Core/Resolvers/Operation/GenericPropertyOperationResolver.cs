using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericPropertyOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolve(InspectorProperty property)
        {
            return property.Info.MemberInfo != null;
        }

        protected override void Initialize()
        {
            var propertyOperation = GetPropertyOperation();

            if (Property.ChildrenResolver is ICollectionStructureResolver)
            {
                var collectionOperation = GetCollectionOperation();
                var collectionType = Property.ValueEntry.ValueType;
                var elementType = collectionType.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];
                if (collectionOperation is IOrderedCollectionOperation)
                {
                    var operationWrapperType = typeof(OrderedCollectionOperationWrapper<,>).MakeGenericType(collectionType, elementType);
                    _operation = operationWrapperType.CreateInstance<ICollectionOperation>(propertyOperation, collectionOperation);
                }
                else
                {
                    var operationWrapperType = typeof(CollectionOperationWrapper<,>).MakeGenericType(collectionType, elementType);
                    _operation = operationWrapperType.CreateInstance<ICollectionOperation>(propertyOperation, collectionOperation);
                }
            }
            else
            {
                _operation = propertyOperation;
            }
        }

        private IPropertyOperation GetPropertyOperation()
        {
            var ownerType = Property.Parent.ValueEntry.ValueType;
            var operationType = typeof(MemberPropertyOperation<,>).MakeGenericType(ownerType, Property.ValueEntry.ValueType);
            return operationType.CreateInstance<IPropertyOperation>(Property.Info.MemberInfo);
        }

        private ICollectionOperation GetCollectionOperation()
        {
            var ownerType = Property.Parent.ValueEntry.ValueType;
            var collectionType = Property.ValueEntry.ValueType;
            var elementType = collectionType.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];

            if (collectionType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, elementType);
                return operationType.CreateInstance<ICollectionOperation>(ownerType);
            }

            if (collectionType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, elementType);
                return operationType.CreateInstance<ICollectionOperation>(ownerType);
            }

            throw new NotSupportedException($"Collection type {collectionType} is not supported.");
        }

        protected override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}

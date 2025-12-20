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
            return property.Info.IsLogicRoot || property.Info.MemberInfo != null;
        }

        protected override void Initialize()
        {
            var propertyOperation = GetPropertyOperation();

            if (Property.BaseValueEntry.ValueType.IsInheritsFrom(typeof(IEnumerable<>)))
            {
                var collectionOperation = GetCollectionOperation();
                var collectionType = Property.BaseValueEntry.ValueType;
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
            if (Property.Info.IsLogicRoot)
            {
                return new GenericPropertyOperation(
                    null, Property.BaseValueEntry.ValueType,
                    (ref object index) => Property.Tree.Targets[(int)index],
                    (ref object index, object value) => throw new InvalidOperationException("Cannot set logic root"));
            }
            else
            {
                var ownerType = Property.Parent.ValueEntry.ValueType;
                var operationType = typeof(MemberPropertyOperation<,>).MakeGenericType(ownerType, Property.BaseValueEntry.ValueType);
                return operationType.CreateInstance<IPropertyOperation>(Property.Info.MemberInfo);
            }
        }

        private ICollectionOperation GetCollectionOperation()
        {
            var ownerType = Property.Parent.ValueEntry.ValueType;
            var collectionType = Property.BaseValueEntry.ValueType;
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

        public override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}

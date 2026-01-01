using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericValueOperationResolver : ValueOperationResolverBase
    {
        private IValueOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Roles.IsValue() && !element.Definition.Roles.IsCustomValue();
        }

        protected override void Initialize()
        {
            var propertyOperation = GetPropertyOperation();

            if (Element is ICollectionElement collectionElement)
            {
                var collectionOperation = GetCollectionOperation();
                var collectionType = collectionElement.ValueEntry.ValueType;
                if (collectionOperation is IOrderedCollectionOperation)
                {
                    var operationWrapperType = typeof(OrderedCollectionOperationWrapper<,>).MakeGenericType(collectionType, collectionElement.Definition.ItemType);
                    _operation = operationWrapperType.CreateInstance<ICollectionOperation>(propertyOperation, collectionOperation);
                }
                else
                {
                    var operationWrapperType = typeof(CollectionOperationWrapper<,>).MakeGenericType(collectionType, collectionElement.Definition.ItemType);
                    _operation = operationWrapperType.CreateInstance<ICollectionOperation>(propertyOperation, collectionOperation);
                }
            }
            else
            {
                _operation = propertyOperation;
            }
        }

        private IValueOperation GetPropertyOperation()
        {
            var ownerType = Element.LogicalParent.CastValue().ValueEntry.ValueType;
            var operationType = typeof(MemberValueOperation<,>).MakeGenericType(ownerType, Element.ValueEntry.ValueType);
            return operationType.CreateInstance<IValueOperation>(((IMemberDefinition)Element.Definition).MemberInfo);
        }

        private ICollectionOperation GetCollectionOperation()
        {
            var collectionElement = (ICollectionElement)Element;
            var ownerType = collectionElement.LogicalParent.CastValue().ValueEntry.ValueType;
            var collectionType = collectionElement.ValueEntry.ValueType;

            if (collectionType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, collectionElement.Definition.ItemType);
                return operationType.CreateInstance<ICollectionOperation>(ownerType);
            }

            if (collectionType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                var operationType = typeof(ListOperation<,>).MakeGenericType(collectionType, collectionElement.Definition.ItemType);
                return operationType.CreateInstance<ICollectionOperation>(ownerType);
            }

            throw new NotSupportedException($"Collection type {collectionType} is not supported.");
        }

        protected override IValueOperation GetOperation()
        {
            return _operation;
        }

        /// <summary>
        /// Clears the cached operation when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _operation = null;
        }
    }
}

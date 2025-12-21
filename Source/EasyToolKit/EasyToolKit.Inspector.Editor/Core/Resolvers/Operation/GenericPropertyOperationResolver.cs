using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericPropertyOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Flags.IsValue();
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

        private IPropertyOperation GetPropertyOperation()
        {
            var ownerType = Element.Parent.ValueEntry.ValueType;
            Type operationType = null;
            if (Element.Definition.Flags.IsMember())
            {
                operationType = typeof(MemberPropertyOperation<,>).MakeGenericType(ownerType, Element.ValueEntry.ValueType);
            }
            else
            {
                throw new NotImplementedException();
            }
            return operationType.CreateInstance<IPropertyOperation>(((IMemberDefinition)Element.Definition).MemberInfo);
        }

        private ICollectionOperation GetCollectionOperation()
        {
            var collectionElement = (ICollectionElement)Element;
            var ownerType = collectionElement.Parent.ValueEntry.ValueType;
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

        protected override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}

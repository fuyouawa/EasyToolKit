using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class CollectionItemOperationResolver : ValueOperationResolverBase
    {
        private IValueOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Roles.IsCollectionItem();
        }

        protected override void Initialize()
        {
            var itemElement = (ICollectionItemElement)Element;
            var collectionType = Element.LogicalParent.CastValue().ValueEntry.ValueType;
            var valueType = Element.Definition.ValueType;

            Type operationType;
            if (collectionType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                operationType = typeof(ListElementOperation<,>).MakeGenericType(collectionType, valueType);
            }
            else if (collectionType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                operationType = typeof(ReadOnlyListElementOperation<,>).MakeGenericType(collectionType, valueType);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported collection type '{valueType}'.");
            }

            _operation = operationType.CreateInstance<IValueOperation>(itemElement.Definition.ItemIndex);
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

using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class CollectionItemOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Flags.IsCollectionItem();
        }

        protected override void Initialize()
        {
            var itemElement = (ICollectionItemElement)Element;
            var collectionType = Element.Parent.ValueEntry.ValueType;
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

            _operation = operationType.CreateInstance<IPropertyOperation>(itemElement.Definition.ItemIndex);
        }

        protected override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}

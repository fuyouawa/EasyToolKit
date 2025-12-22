using System;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.Inspector.Editor
{
    public static class ElementUtility
    {
        public static Type GetOwnerTypeWithAttribute(IElement element, Attribute attribute)
        {
            var source = element.GetAttributeInfo(attribute)?.Source;
            if (source == null)
            {
                throw new ArgumentException($"Attribute '{attribute.GetType()}' not found in the element '{element}'");
            }

            switch (source)
            {
                case ElementAttributeSource.Type:
                    var valueElement = (IValueElement)element;
                    return valueElement.ValueEntry.ValueType;
                case ElementAttributeSource.Member:
                case ElementAttributeSource.ListPassToElement:
                    return element.LogicalParent!.ValueEntry.ValueType;
                default:
                    throw new IndexOutOfRangeException();
            }
        }


        public static object GetOwnerWithAttribute(IElement element, Attribute attribute, int targetIndex)
        {
            var source = element.GetAttributeInfo(attribute)?.Source;
            if (source == null)
            {
                throw new ArgumentException($"Attribute '{attribute.GetType()}' not found in the element '{element}'");
            }

            switch (source)
            {
                case ElementAttributeSource.Type:
                    var valueElement = (IValueElement)element;
                    return valueElement.ValueEntry.GetWeakValue(targetIndex);
                case ElementAttributeSource.Member:
                case ElementAttributeSource.ListPassToElement:
                    return element.LogicalParent!.ValueEntry.GetWeakValue(targetIndex);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static string GetKey(IElement element)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(element.SharedContext.Tree.Targets[0].GetType());
            var key2 = element.Path;
            return string.Join("+", key1, key2);
        }
    }
}

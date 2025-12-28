using System;
using EasyToolKit.ThirdParty.OdinSerializer;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class ElementUtility
    {
        public static Type GetOwnerTypeWithAttribute([NotNull] ILogicalElement element, [NotNull] Attribute attribute)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

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
                    return element.LogicalParent.CastValue().ValueEntry.ValueType;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static object GetOwnerWithAttribute([NotNull] ILogicalElement element, [NotNull] Attribute attribute)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            var source = element.GetAttributeInfo(attribute)?.Source;
            if (source == null)
            {
                throw new ArgumentException($"Attribute '{attribute.GetType()}' not found in the element '{element}'");
            }

            switch (source)
            {
                case ElementAttributeSource.Type:
                    var valueElement = (IValueElement)element;
                    return valueElement.ValueEntry.WeakSmartValue;
                case ElementAttributeSource.Member:
                case ElementAttributeSource.ListPassToElement:
                    return element.LogicalParent.CastValue().ValueEntry.WeakSmartValue;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static object GetOwnerWithAttribute([NotNull] ILogicalElement element, [NotNull] Attribute attribute, int targetIndex)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

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
                    return element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static string GetKey(IElement element)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(element.SharedContext.Tree.TargetType);
            var key2 = element.Path;
            return string.Join("+", key1, key2);
        }

        public static IElementList<IElement> GetParentCollection([NotNull] IElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            if (element.Parent == null)
            {
                throw new ArgumentException($"Element '{element}' has no parent collection.", nameof(element));
            }

            if (element.Parent is IValueElement valueElement)
            {
                return valueElement.Children;
            }

            if (element.Parent is IGroupElement groupElement)
            {
                return groupElement.Children;
            }

            if (element.Parent is IMethodElement methodElement)
            {
                return methodElement.Children;
            }

            throw new ArgumentException($"Element '{element}' has no parent collection.", nameof(element));
        }
    }
}

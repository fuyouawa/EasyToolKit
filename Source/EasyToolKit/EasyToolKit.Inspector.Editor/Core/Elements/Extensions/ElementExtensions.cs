using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public static class ElementExtensions
    {
        public static IValueElement AsValue(this IElement element)
        {
            return element as IValueElement;
        }

        public static ElementAttributeInfo GetAttributeInfo(this IElement element, Attribute matchedAttribute)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (ReferenceEquals(attributeInfo.Attribute, matchedAttribute))
                {
                    return attributeInfo;
                }
            }

            return null;
        }


        public static ElementAttributeInfo GetAttributeInfo(this IElement element, Type attributeType)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (attributeInfo.Attribute.GetType() == attributeType)
                {
                    return attributeInfo;
                }
            }

            return null;
        }

        public static TAttribute GetAttribute<TAttribute>(this IElement element) where TAttribute : Attribute
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (attributeInfo.Attribute is TAttribute attribute)
                {
                    return attribute;
                }
            }

            return null;
        }

        public static IEnumerable<Attribute> EnumerateAttributes(this IElement element)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                yield return attributeInfo.Attribute;
            }
        }
    }
}

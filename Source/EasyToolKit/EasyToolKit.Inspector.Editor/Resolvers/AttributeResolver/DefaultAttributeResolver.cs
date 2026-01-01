using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeResolver : AttributeResolverBase
    {
        private ElementAttributeInfo[] _attributeInfos;

        // protected override bool CanResolve(IElement element)
        // {
        //     return !element.Definition.Roles.IsGroup();
        // }

        protected override void Initialize()
        {
            var attributeInfos = new List<ElementAttributeInfo>();

            if (Element.Definition.Roles.IsCollectionItem() && Element is ILogicalElement logicalElement && logicalElement.LogicalParent != null)
            {
                var passToListElementAttributes = logicalElement.LogicalParent.EnumerateAttributes()
                    .Where(attr => attr is CanPassToListElementAttribute { PassToListElements: true });
                foreach (var attribute in passToListElementAttributes)
                {
                    attributeInfos.Add(new ElementAttributeInfo(attribute, ElementAttributeSource.ListPassToElement));
                }
            }

            if (Element.Definition is IMemberDefinition memberDefinition)
            {
                var memberAttributes = memberDefinition.MemberInfo.GetCustomAttributes();
                foreach (var attribute in memberAttributes)
                {
                    attributeInfos.Add(new ElementAttributeInfo(attribute, ElementAttributeSource.Member));
                }
            }

            if (Element is IValueElement valueElement)
            {
                var typeAttributes = valueElement.ValueEntry.ValueType.GetCustomAttributes(true).Cast<Attribute>();
                foreach (var attribute in typeAttributes)
                {
                    attributeInfos.Add(new ElementAttributeInfo(attribute, ElementAttributeSource.Type));
                }
            }

            if (Element.Definition.AdditionalAttributes != null)
            {
                attributeInfos.AddRange(Element.Definition.AdditionalAttributes
                    .Select(attribute => new ElementAttributeInfo(attribute, ElementAttributeSource.Custom)));
            }

            _attributeInfos = attributeInfos.ToArray();
        }

        protected override ElementAttributeInfo[] GetAttributeInfos()
        {
            return _attributeInfos;
        }

        /// <summary>
        /// Clears the cached attribute infos when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _attributeInfos = null;
        }
    }
}

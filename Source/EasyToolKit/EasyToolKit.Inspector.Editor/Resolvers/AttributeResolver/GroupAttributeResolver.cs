using System;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class GroupAttributeResolver : AttributeResolverBase
    {
        protected override bool CanResolve(IElement element)
        {
            return element.Definition.Roles.IsGroup();
        }

        protected override ElementAttributeInfo[] GetAttributeInfos()
        {
            var element = (IGroupElement)Element;
            if (element.AssociatedElement != null)
            {
                return element.AssociatedElement.GetAttributeInfos().ToArray();
            }

            return Array.Empty<ElementAttributeInfo>();
        }
    }
}

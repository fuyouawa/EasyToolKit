using System;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class GroupAttributeResolver : AttributeResolverBase
    {
        private ElementAttributeInfo[] _attributeInfos;

        protected override void Initialize()
        {
            _attributeInfos = Element.LogicalChildren[0].GetAttributeInfos().ToArray();
        }

        protected override ElementAttributeInfo[] GetAttributeInfos()
        {
            return _attributeInfos;
        }
    }
}

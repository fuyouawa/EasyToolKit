using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultPropertyStructureResolverFactory : IPropertyStructureResolverFactory
    {
        public IPropertyStructureResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = InspectorResolverUtility.GetResolverType(property, typeof(IPropertyStructureResolver));
            return resolverType.CreateInstance<IPropertyStructureResolver>();
        }
    }
}

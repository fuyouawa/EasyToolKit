using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultPropertyStructureResolverFactory : IPropertyStructureResolverFactory
    {
        public IValueStructureResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = ResolverUtility.GetResolverType(property, typeof(IValueStructureResolver));
            return resolverType.CreateInstance<IValueStructureResolver>();
        }
    }
}

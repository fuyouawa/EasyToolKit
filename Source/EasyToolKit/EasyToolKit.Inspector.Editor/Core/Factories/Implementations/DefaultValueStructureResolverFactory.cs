using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultValueStructureResolverFactory : IValueStructureResolverFactory
    {
        public IValueStructureResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IValueStructureResolver));
            return resolverType.CreateInstance<IValueStructureResolver>();
        }
    }
}

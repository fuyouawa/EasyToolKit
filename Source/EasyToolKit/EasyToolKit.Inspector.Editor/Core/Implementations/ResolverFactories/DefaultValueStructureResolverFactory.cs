using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultValueStructureResolverFactory : IValueStructureResolverFactory
    {
        public IStructureResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IStructureResolver));
            return resolverType.CreateInstance<IStructureResolver>();
        }
    }
}

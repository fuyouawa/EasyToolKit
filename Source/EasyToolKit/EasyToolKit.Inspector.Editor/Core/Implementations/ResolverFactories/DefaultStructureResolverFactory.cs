using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultStructureResolverFactory : IStructureResolverFactory
    {
        public IStructureResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IStructureResolver));
            return resolverType != null ? ResolverUtility.RentResolver(resolverType) as IStructureResolver : null;
        }
    }
}

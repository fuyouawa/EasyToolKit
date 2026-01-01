using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultDrawerChainResolverFactory : IDrawerChainResolverFactory
    {
        public IDrawerChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IDrawerChainResolver));
            return resolverType != null ? ResolverUtility.RentResolver(resolverType) as IDrawerChainResolver : null;
        }
    }
}

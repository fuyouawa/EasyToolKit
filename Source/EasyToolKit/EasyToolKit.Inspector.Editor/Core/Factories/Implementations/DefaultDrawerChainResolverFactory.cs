using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChainResolverFactory : IDrawerChainResolverFactory
    {
        public IDrawerChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IDrawerChainResolver));
            return resolverType.CreateInstance<IDrawerChainResolver>();
        }
    }
}

using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultDrawerChainResolverFactory : IDrawerChainResolverFactory
    {
        public IDrawerChainResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = InspectorResolverUtility.GetResolverType(property, typeof(IDrawerChainResolver));
            return resolverType.CreateInstance<IDrawerChainResolver>();
        }
    }
}
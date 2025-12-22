using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeResolverFactory : IAttributeResolverFactory
    {
        public IAttributeResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IAttributeResolver));
            return resolverType.CreateInstance<IAttributeResolver>();
        }
    }
}

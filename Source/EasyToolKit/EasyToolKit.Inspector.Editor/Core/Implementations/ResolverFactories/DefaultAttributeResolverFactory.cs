using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultAttributeResolverFactory : IAttributeResolverFactory
    {
        public IAttributeResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IAttributeResolver));
            return resolverType != null ? ResolverUtility.RentResolver(resolverType) as IAttributeResolver : null;
        }
    }
}

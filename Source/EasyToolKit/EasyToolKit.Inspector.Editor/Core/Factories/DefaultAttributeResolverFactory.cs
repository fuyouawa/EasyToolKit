using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeResolverFactory : IAttributeResolverFactory
    {
        public IAttributeResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = ResolverUtility.GetResolverType(property, typeof(IAttributeResolver));
            return resolverType.CreateInstance<IAttributeResolver>();
        }
    }
}
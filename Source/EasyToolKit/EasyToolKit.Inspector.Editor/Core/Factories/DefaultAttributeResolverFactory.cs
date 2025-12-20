using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeResolverFactory : IAttributeResolverFactory
    {
        public IAttributeResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = InspectorResolverUtility.GetResolverType(property, typeof(IAttributeResolver));
            return resolverType.CreateInstance<IAttributeResolver>();
        }
    }
}
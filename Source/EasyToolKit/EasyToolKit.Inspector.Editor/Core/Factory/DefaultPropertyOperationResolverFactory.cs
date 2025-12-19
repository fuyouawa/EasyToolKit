using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultPropertyOperationResolverFactory : IPropertyOperationResolverFactory
    {
        public IPropertyOperationResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = InspectorResolverUtility.GetResolverType(property, typeof(IPropertyOperationResolver));
            return resolverType.CreateInstance<IPropertyOperationResolver>();
        }
    }
}
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultPropertyOperationResolverFactory : IPropertyOperationResolverFactory
    {
        public IValueOperationResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = ResolverUtility.GetResolverType(property, typeof(IValueOperationResolver));
            return resolverType.CreateInstance<IValueOperationResolver>();
        }
    }
}
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultGroupResolverFactory : IGroupResolverFactory
    {
        public IGroupResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = InspectorResolverUtility.GetResolverType(property, typeof(IGroupResolver));
            return resolverType.CreateInstance<IGroupResolver>();
        }
    }
}
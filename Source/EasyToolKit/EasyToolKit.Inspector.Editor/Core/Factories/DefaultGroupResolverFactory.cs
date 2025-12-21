using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultGroupResolverFactory : IGroupResolverFactory
    {
        public IGroupResolver CreateResolver(InspectorProperty property)
        {
            var resolverType = ResolverUtility.GetResolverType(property, typeof(IGroupResolver));
            return resolverType.CreateInstance<IGroupResolver>();
        }
    }
}
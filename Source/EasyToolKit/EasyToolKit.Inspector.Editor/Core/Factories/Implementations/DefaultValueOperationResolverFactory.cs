using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultValueOperationResolverFactory : IValueOperationResolverFactory
    {
        public IValueOperationResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IValueOperationResolver));
            return resolverType.CreateInstance<IValueOperationResolver>();
        }
    }
}

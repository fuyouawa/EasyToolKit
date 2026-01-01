using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DefaultValueOperationResolverFactory : IValueOperationResolverFactory
    {
        public IValueOperationResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IValueOperationResolver));
            return resolverType != null ? ResolverUtility.RentResolver(resolverType) as IValueOperationResolver : null;
        }
    }
}

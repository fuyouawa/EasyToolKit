using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IPostProcessorChainResolverFactory"/>
    /// </summary>
    public class DefaultPostProcessorChainResolverFactory : IPostProcessorChainResolverFactory
    {
        /// <summary>
        /// Creates a post processor chain resolver for the specified element
        /// </summary>
        /// <param name="element">The element to create the resolver for</param>
        /// <returns>The created post processor chain resolver</returns>
        public IPostProcessorChainResolver CreateResolver(IElement element)
        {
            var resolverType = ResolverUtility.GetResolverType(element, typeof(IPostProcessorChainResolver));
            return resolverType != null ? ResolverUtility.RentResolver(resolverType) as IPostProcessorChainResolver : null;
        }
    }
}

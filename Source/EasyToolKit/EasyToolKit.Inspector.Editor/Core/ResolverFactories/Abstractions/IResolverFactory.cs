using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public interface IResolverFactory<TResolver> where TResolver : IResolver
    {
        [CanBeNull] TResolver CreateResolver(IElement element);
    }
}

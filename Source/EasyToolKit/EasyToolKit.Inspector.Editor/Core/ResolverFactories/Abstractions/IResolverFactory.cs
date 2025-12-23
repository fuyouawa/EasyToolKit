namespace EasyToolKit.Inspector.Editor
{
    public interface IResolverFactory<TResolver> where TResolver : IResolver
    {
        TResolver CreateResolver(IElement element);
    }
}

namespace EasyToolKit.Inspector.Editor
{
    public interface IResolverFactory
    {
        IResolver CreateResolver(IElement element);
    }
}

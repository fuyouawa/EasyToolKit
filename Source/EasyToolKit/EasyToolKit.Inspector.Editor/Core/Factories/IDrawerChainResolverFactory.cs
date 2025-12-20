namespace EasyToolKit.Inspector.Editor
{
    public interface IDrawerChainResolverFactory
    {
        IDrawerChainResolver CreateResolver(InspectorProperty property);
    }
}
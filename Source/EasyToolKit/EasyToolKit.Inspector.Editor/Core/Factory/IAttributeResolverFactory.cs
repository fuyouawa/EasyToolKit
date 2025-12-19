namespace EasyToolKit.Inspector.Editor
{
    public interface IAttributeResolverFactory
    {
        IAttributeResolver CreateResolver(InspectorProperty property);
    }
}
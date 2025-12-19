namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyStructureResolverFactory
    {
        IPropertyStructureResolver CreateResolver(InspectorProperty property);
    }
}

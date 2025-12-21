namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyStructureResolverFactory
    {
        IValueStructureResolver CreateResolver(InspectorProperty property);
    }
}

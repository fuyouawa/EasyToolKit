namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyStructureResolverLocatorFactory
    {
        IPropertyStructureResolverLocator CreateLocator(InspectorProperty property);
    }
}

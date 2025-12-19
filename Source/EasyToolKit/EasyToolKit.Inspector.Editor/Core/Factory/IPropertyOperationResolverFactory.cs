namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolverFactory
    {
        IPropertyOperationResolver CreateResolver(InspectorProperty property);
    }
}

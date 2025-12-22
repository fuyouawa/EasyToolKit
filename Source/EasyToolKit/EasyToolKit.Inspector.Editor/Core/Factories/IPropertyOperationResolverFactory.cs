namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolverFactory
    {
        IValueOperationResolver CreateResolver(InspectorProperty property);
    }
}

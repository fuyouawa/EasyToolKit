namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolver
    {
        IPropertyOperation GetOperation(InspectorProperty property);
    }
}

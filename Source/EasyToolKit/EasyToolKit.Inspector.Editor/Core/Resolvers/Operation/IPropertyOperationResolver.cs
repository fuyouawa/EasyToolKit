namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolver : IInspectorHandler
    {
        IPropertyOperation GetOperation();
    }
}

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolver : IInspectorElement
    {
        IPropertyOperation GetOperation();
    }
}

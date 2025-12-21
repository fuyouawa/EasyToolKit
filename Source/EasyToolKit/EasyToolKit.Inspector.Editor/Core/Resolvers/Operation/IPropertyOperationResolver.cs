namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyOperationResolver : IHandler
    {
        IPropertyOperation GetOperation();
    }
}

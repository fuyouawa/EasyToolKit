namespace EasyToolKit.Inspector.Editor
{
    public interface IValueOperationResolver : IHandler
    {
        IValueOperation GetOperation();
    }
}

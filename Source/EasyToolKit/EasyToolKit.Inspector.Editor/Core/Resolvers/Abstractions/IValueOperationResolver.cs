namespace EasyToolKit.Inspector.Editor
{
    public interface IValueOperationResolver : IResolver
    {
        IValueOperation GetOperation();
    }
}

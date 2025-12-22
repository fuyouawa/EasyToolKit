namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionElementOperation : IValueOperation
    {
        int ElementIndex { get; }
    }

    public interface ICollectionElementOperation<TValue> : ICollectionElementOperation, IValueOperation<TValue>
    {
    }
}

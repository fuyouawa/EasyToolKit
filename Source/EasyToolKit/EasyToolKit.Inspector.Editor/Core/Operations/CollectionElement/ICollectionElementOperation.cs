namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionElementOperation : IPropertyOperation
    {
        int ElementIndex { get; }
    }

    public interface ICollectionElementOperation<TValue> : ICollectionElementOperation, IPropertyOperation<TValue>
    {
    }
}

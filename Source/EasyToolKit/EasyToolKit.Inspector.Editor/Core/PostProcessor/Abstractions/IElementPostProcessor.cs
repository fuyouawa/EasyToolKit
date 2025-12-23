namespace EasyToolKit.Inspector.Editor
{
    public interface IElementPostProcessor : IHandler
    {
        void Process(IElement element);
    }
}

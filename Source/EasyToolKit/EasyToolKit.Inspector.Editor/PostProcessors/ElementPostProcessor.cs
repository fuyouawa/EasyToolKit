namespace EasyToolKit.Inspector.Editor
{
    public abstract class ElementPostProcessor : IElementPostProcessor
    {
        protected virtual bool CanProcess(IElement element)
        {
            return true;
        }

        protected abstract void Process(IElement element);

        void IElementPostProcessor.Process(IElement element)
        {
            Process(element);
        }

        bool IHandler.CanHandle(IElement element)
        {
            return CanProcess(element);
        }
    }
}

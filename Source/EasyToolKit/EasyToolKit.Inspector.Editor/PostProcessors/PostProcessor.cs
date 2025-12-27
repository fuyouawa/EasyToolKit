namespace EasyToolKit.Inspector.Editor
{
    public abstract class PostProcessor : IPostProcessor
    {
        private IElement _element;
        private PostProcessorChain _chain;

        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }

        PostProcessorChain IPostProcessor.Chain
        {
            get => _chain;
            set => _chain = value;
        }

        public IElement Element => _element;
        public PostProcessorChain Chain => _chain;

        protected virtual bool CanProcess(IElement element)
        {
            return true;
        }

        protected abstract void Process();


        protected bool CallNextProcessor()
        {
            if (_chain.MoveNext() && _chain.Current != null)
            {
                _chain.Current.Process();
                return true;
            }
            return false;
        }

        void IPostProcessor.Process()
        {
            Process();
        }

        bool IHandler.CanHandle(IElement element)
        {
            return CanProcess(element);
        }
    }
}

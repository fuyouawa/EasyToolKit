namespace EasyToolKit.Inspector.Editor
{
    public class ResolverBase : IResolver
    {
        private IElement _element;

        IElement IResolver.Element
        {
            get => _element;
            set => _element = value;
        }

        public IElement Element => _element;

        protected virtual bool CanResolve(IElement element) => true;

        bool IHandler.CanHandle(IElement element)
        {
            return CanResolve(element);
        }
    }
}

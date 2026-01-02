using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class ResolverBase : IResolver
    {
        private IElement _element;

        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }

        public IElement Element => _element;

        protected virtual bool CanResolve(IElement element) => true;

        protected virtual void OnRent()
        {
        }

        protected virtual void OnRelease()
        {
            _element = null;
        }

        bool IHandler.CanHandle(IElement element)
        {
            return CanResolve(element);
        }

        void IPoolItem.Rent()
        {
            OnRent();
        }

        void IPoolItem.Release()
        {
            OnRelease();
        }
    }
}

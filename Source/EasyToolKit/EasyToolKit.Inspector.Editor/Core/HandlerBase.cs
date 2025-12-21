using System;

namespace EasyToolKit.Inspector.Editor
{
    public class HandlerBase : IHandler
    {
        private IElement _element;

        IElement IHandler.Element
        {
            get => _element;
            set => _element = value;
        }
        public IElement Element => _element;

        bool IHandler.CanHandle(IElement element)
        {
            return CanHandle(element);
        }

        protected virtual bool CanHandle(IElement element)
        {
            return true;
        }
    }
}

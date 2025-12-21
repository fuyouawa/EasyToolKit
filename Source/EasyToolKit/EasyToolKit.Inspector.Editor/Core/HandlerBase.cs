using System;

namespace EasyToolKit.Inspector.Editor
{
    public class HandlerBase : IHandler
    {
        private InspectorProperty _property;
        private bool _isInitialized;

        InspectorProperty IHandler.Property
        {
            get => _property;
            set => _property = value;
        }
        public InspectorProperty Property => _property;

        bool IHandler.CanHandle(InspectorProperty property)
        {
            return CanHandle(property);
        }

        protected virtual bool CanHandle(InspectorProperty property)
        {
            return true;
        }
    }
}

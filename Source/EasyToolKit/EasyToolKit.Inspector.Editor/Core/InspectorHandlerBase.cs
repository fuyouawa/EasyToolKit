using System;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorHandlerBase : IInspectorHandler
    {
        private InspectorProperty _property;
        private bool _isInitialized;

        InspectorProperty IInspectorHandler.Property
        {
            get => _property;
            set => _property = value;
        }
        public InspectorProperty Property => _property;

        bool IInspectorHandler.CanHandle(InspectorProperty property)
        {
            return CanHandle(property);
        }

        protected virtual bool CanHandle(InspectorProperty property)
        {
            return true;
        }
    }
}

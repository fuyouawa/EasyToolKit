using System;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorElementBase : IInspectorElement
    {
        private InspectorProperty _property;
        private bool _isInitialized;

        InspectorProperty IInspectorElement.Property
        {
            get => _property;
            set => _property = value;
        }

        public bool IsInitialized => _isInitialized;
        public InspectorProperty Property => _property;

        bool IInspectorElement.CanHandle(InspectorProperty property)
        {
            return CanHandle(property);
        }

        protected virtual bool CanHandle(InspectorProperty property)
        {
            return true;
        }

        void IInspectorElement.Initialize()
        {
            if (_isInitialized) return;
            Initialize();
            _isInitialized = true;
        }

        protected virtual void Initialize()
        {
        }
    }
}

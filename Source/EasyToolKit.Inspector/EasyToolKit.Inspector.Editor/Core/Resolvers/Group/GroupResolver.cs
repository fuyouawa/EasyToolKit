using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IGroupResolver : IInitializableResolver
    {
        InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }

    public abstract class GroupResolver : IGroupResolver
    {
        public InspectorProperty Property { get; private set; }

        public bool IsInitialized { get; private set; }

        InspectorProperty IInitializableResolver.Property
        {
            get => Property;
            set => Property = value;
        }

        bool IInitializable.IsInitialized => IsInitialized;

        void IInitializable.Initialize()
        {
            if (IsInitialized) return;
            Initialize();
            IsInitialized = true;
        }

        void IInitializable.Deinitialize()
        {
            if (!IsInitialized) return;
            Deinitialize();
            IsInitialized = false;
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Deinitialize()
        {
        }

        public abstract InspectorProperty[] GetGroupProperties(Type beginGroupAttributeType);
    }
}

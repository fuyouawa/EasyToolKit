using System;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyResolver : IInitializableResolver
    {
        int ChildCount { get; }
        InspectorPropertyInfo GetChildInfo(int childIndex);
        int ChildNameToIndex(string name);
        bool ApplyChanges();
    }

    public abstract class PropertyResolver : IPropertyResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;
        public InspectorProperty Property { get; private set; }
        public bool IsInitialized { get; private set; }

        InspectorProperty IInitializableResolver.Property
        {
            get => Property;
            set => Property = value;
        }

        bool IInitializable.IsInitialized => IsInitialized;

        public int ChildCount
        {
            get
            {
                if (_lastChildCountUpdateId != Property.Tree.UpdateId)
                {
                    _lastChildCountUpdateId = Property.Tree.UpdateId;
                    _childCount = CalculateChildCount();
                }
                return _childCount;
            }
        }

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

        protected virtual void Initialize() { }
        protected virtual void Deinitialize() { }

        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);
        public abstract int ChildNameToIndex(string name);
        public abstract int CalculateChildCount();

        bool IPropertyResolver.ApplyChanges()
        {
            return ApplyChanges();
        }

        protected virtual bool ApplyChanges() => false;
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property structure resolvers in the inspector system.
    /// Provides common functionality for property structure resolution without collection operations.
    /// </summary>
    public abstract class PropertyStructureResolverBase : IPropertyStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;

        /// <summary>
        /// Gets the InspectorProperty associated with this resolver
        /// </summary>
        public InspectorProperty Property { get; private set; }

        /// <summary>
        /// Gets whether this resolver has been initialized
        /// </summary>
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

        /// <summary>
        /// Initializes the resolver (can be overridden by derived classes)
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// Deinitializes the resolver (can be overridden by derived classes)
        /// </summary>
        protected virtual void Deinitialize() { }

        public virtual bool CanResolver(InspectorProperty property) => true;

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        public abstract int ChildNameToIndex(string name);

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        protected abstract int CalculateChildCount();
    }

    public abstract class PropertyStructureResolverBase<T> : PropertyStructureResolverBase
    {
        private IPropertyValueEntry<T> _valueEntry;

        /// <summary>
        /// Gets the strongly-typed value entry for the property.
        /// The value entry is lazily loaded and will attempt to update the property if not found initially.
        /// </summary>
        public IPropertyValueEntry<T> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<T>;

                    if (_valueEntry == null)
                    {
                        Property.Update(true);
                        _valueEntry = Property.ValueEntry as IPropertyValueEntry<T>;
                    }
                }

                return _valueEntry;
            }
        }

        public override bool CanResolver(InspectorProperty property)
        {
            if (property.ValueEntry == null)
            {
                return false;
            }

            var valueType = property.ValueEntry.ValueType;
            return valueType == typeof(T) &&
                   CanResolverType(valueType) &&
                   CanResolverProperty(property);
        }

        protected virtual bool CanResolverType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanResolverProperty(InspectorProperty property)
        {
            return true;
        }
    }
}

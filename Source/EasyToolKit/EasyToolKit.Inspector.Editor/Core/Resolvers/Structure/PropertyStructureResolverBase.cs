using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property structure resolvers in the inspector system.
    /// Provides common functionality for property structure resolution without collection operations.
    /// </summary>
    public abstract class PropertyStructureResolverBase : InspectorResolverBase, IPropertyStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;
        private bool _isInitialized;

        public int ChildCount
        {
            get
            {
                EnsureInitialize();
                if (_lastChildCountUpdateId != Property.Tree.UpdateId)
                {
                    _lastChildCountUpdateId = Property.Tree.UpdateId;
                    _childCount = CalculateChildCount();
                }
                return _childCount;
            }
        }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        protected abstract InspectorPropertyInfo GetChildInfo(int childIndex);

        /// <summary>
        /// Converts a child property name to its index
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>The index of the child property, or -1 if not found</returns>
        protected abstract int ChildNameToIndex(string name);

        /// <summary>
        /// Calculates the number of child properties
        /// </summary>
        /// <returns>The number of child properties</returns>
        protected abstract int CalculateChildCount();

        protected virtual void Initialize()
        {
        }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        InspectorPropertyInfo IPropertyStructureResolver.GetChildInfo(int childIndex)
        {
            EnsureInitialize();
            return GetChildInfo(childIndex);
        }

        int IPropertyStructureResolver.ChildNameToIndex(string name)
        {
            EnsureInitialize();
            return ChildNameToIndex(name);
        }
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

        protected override bool CanResolve(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;
            return valueType == typeof(T) &&
                   CanResolveType(valueType) &&
                   CanResolveProperty(property);
        }

        protected virtual bool CanResolveType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanResolveProperty(InspectorProperty property)
        {
            return true;
        }
    }
}

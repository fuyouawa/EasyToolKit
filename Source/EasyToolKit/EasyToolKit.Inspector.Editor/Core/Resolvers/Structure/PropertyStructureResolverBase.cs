using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property structure resolvers in the inspector system.
    /// Provides common functionality for property structure resolution without collection operations.
    /// </summary>
    public abstract class PropertyStructureResolverBase : InspectorElementBase, IPropertyStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;

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

        public override Type MatchedType => typeof(PropertyStructureResolverBase<>);

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

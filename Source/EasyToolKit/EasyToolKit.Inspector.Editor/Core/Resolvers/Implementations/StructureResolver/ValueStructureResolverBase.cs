using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for value structure resolvers in the inspector system.
    /// Provides common functionality for value structure resolution without collection operations.
    /// </summary>
    public abstract class ValueStructureResolverBase : ResolverBase, IValueStructureResolver
    {
        private int? _lastChildCountUpdateId;
        private int _childCount;
        private bool _isInitialized;

        new public IValueElement Element => base.Element as IValueElement;

        public int ChildCount
        {
            get
            {
                EnsureInitialize();
                if (_lastChildCountUpdateId != Element.SharedContext.UpdateId)
                {
                    _lastChildCountUpdateId = Element.SharedContext.UpdateId;
                    _childCount = CalculateChildCount();
                }
                return _childCount;
            }
        }

        /// <summary>
        /// Gets the definition of a child at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child</param>
        /// <returns>Definition of the child</returns>
        protected abstract IElementDefinition GetChildDefinition(int childIndex);

        /// <summary>
        /// Converts a child name to its index
        /// </summary>
        /// <param name="name">The name of the child</param>
        /// <returns>The index of the child, or -1 if not found</returns>
        protected abstract int ChildNameToIndex(string name);

        /// <summary>
        /// Calculates the number of children
        /// </summary>
        /// <returns>The number of children</returns>
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

        IElementDefinition IValueStructureResolver.GetChildDefinition(int childIndex)
        {
            EnsureInitialize();
            return GetChildDefinition(childIndex);
        }

        int IValueStructureResolver.ChildNameToIndex(string name)
        {
            EnsureInitialize();
            return ChildNameToIndex(name);
        }
    }

    public abstract class ValueStructureResolverBase<T> : ValueStructureResolverBase
    {
        private IValueEntry<T> _valueEntry;

        /// <summary>
        /// Gets the strongly-typed value entry for the property.
        /// The value entry is lazily loaded and will attempt to update the property if not found initially.
        /// </summary>
        public IValueEntry<T> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Element.ValueEntry as IValueEntry<T>;
                }

                return _valueEntry;
            }
        }

        protected override bool CanResolve(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                var valueType = valueElement.ValueEntry.ValueType;
                return valueType == typeof(T) &&
                       CanResolveType(valueType) &&
                       CanResolveElement(valueElement);
            }

            return false;
        }

        protected virtual bool CanResolveType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanResolveElement(IValueElement element)
        {
            return true;
        }
    }
}

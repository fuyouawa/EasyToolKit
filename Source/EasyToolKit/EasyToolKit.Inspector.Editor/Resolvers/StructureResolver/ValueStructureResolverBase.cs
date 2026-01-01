using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for value structure resolvers in the inspector system.
    /// Provides common functionality for value structure resolution without collection operations.
    /// </summary>
    public abstract class ValueStructureResolverBase : StructureResolverBase
    {
        new public IValueElement Element => base.Element as IValueElement;

        protected override bool CanResolve(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                return CanResolveElement(valueElement);
            }

            return false;
        }

        protected virtual bool CanResolveElement(IValueElement element)
        {
            return true;
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

        /// <summary>
        /// Releases the cached value entry when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _valueEntry = null;
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
    }
}

using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base abstract class for drawers that handle specific value types.
    /// Provides strongly-typed access to property values with type safety.
    /// </summary>
    /// <typeparam name="T">The type of value this drawer handles.</typeparam>
    public abstract class EasyValueDrawer<T> : EasyDrawer
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

        public override Type MatchedType => typeof(EasyValueDrawer<>);

        /// <summary>
        /// Determines whether this drawer can draw the specified property.
        /// This method is sealed and provides the base logic for value type-based property filtering.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        protected sealed override bool CanDrawProperty(InspectorProperty property)
        {
            if (property.ValueEntry == null)
            {
                return false;
            }

            var valueType = property.ValueEntry.ValueType;
            return valueType == typeof(T) &&
                   CanDrawValueType(valueType) &&
                   CanDrawValueProperty(property);
        }

        /// <summary>
        /// Determines whether this drawer can draw properties of the specified value type.
        /// Override this method to restrict which value types this drawer can handle.
        /// </summary>
        /// <param name="valueType">The type of the property value.</param>
        /// <returns>True if this drawer can handle the value type, false otherwise.</returns>
        protected virtual bool CanDrawValueType(Type valueType)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this drawer can draw the specified value property.
        /// Override this method to implement custom value-based filtering logic.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the value property, false otherwise.</returns>
        protected virtual bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }
    }
}

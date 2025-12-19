using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base abstract class for drawers that handle properties decorated with specific attributes.
    /// Provides common functionality for attribute-based property drawing.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute this drawer handles.</typeparam>
    public abstract class EasyAttributeDrawer<TAttribute> : EasyDrawer
        where TAttribute : Attribute
    {
        private TAttribute _attribute;
        private AttributeSource? _attributeSource;

        /// <summary>
        /// Gets the attribute instance that this drawer is handling.
        /// The attribute is lazily loaded from the property when first accessed.
        /// </summary>
        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = Property.GetAttribute<TAttribute>();
                }

                return _attribute;
            }
        }

        /// <summary>
        /// Gets the source location where the attribute was applied.
        /// This indicates whether the attribute was applied to the field, property, or type.
        /// </summary>
        public AttributeSource AttributeSource
        {
            get
            {
                if (_attributeSource == null)
                {
                    _attributeSource = Property.GetAttributeSource(Attribute);
                }
                return _attributeSource.Value;
            }
        }

        public override Type MatchedType => typeof(EasyAttributeDrawer<>);

        /// <summary>
        /// Determines whether this drawer can draw the specified property.
        /// This method is sealed and provides the base logic for attribute-based property filtering.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        protected sealed override bool CanDrawProperty(InspectorProperty property)
        {
            if (property.ValueEntry != null && !CanDrawValueType(property.ValueEntry.ValueType))
            {
                return false;
            }

            return property.GetAttribute<TAttribute>() != null && CanDrawAttributeProperty(property);
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
        /// Determines whether this drawer can draw the specified attribute property.
        /// Override this method to implement custom attribute-based filtering logic.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the attribute property, false otherwise.</returns>
        protected virtual bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return true;
        }
    }

    /// <summary>
    /// Base abstract class for drawers that handle properties decorated with specific attributes and have strongly-typed value access.
    /// Provides common functionality for attribute-based property drawing with type-safe value access.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute this drawer handles.</typeparam>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public abstract class EasyAttributeDrawer<TAttribute, TValue> : EasyAttributeDrawer<TAttribute>
        where TAttribute : Attribute
    {
        private IPropertyValueEntry<TValue> _valueEntry;

        /// <summary>
        /// Gets the strongly-typed value entry for the property.
        /// Provides type-safe access to the property's value.
        /// </summary>
        public IPropertyValueEntry<TValue> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<TValue>;
                }

                return _valueEntry;
            }
        }

        public override Type MatchedType => typeof(EasyAttributeDrawer<,>);


        /// <summary>
        /// Determines whether this drawer can draw the specified attribute property.
        /// This method adds type checking to ensure the property value matches the expected type.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the attribute property, false otherwise.</returns>
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ValueEntry != null &&
                   property.ValueEntry.ValueType == typeof(TValue) &&
                   CanDrawAttributeValueProperty(property);
        }

        /// <summary>
        /// Determines whether this drawer can draw the specified attribute value property.
        /// Override this method to implement custom type-specific filtering logic.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the attribute value property, false otherwise.</returns>
        protected virtual bool CanDrawAttributeValueProperty(InspectorProperty property)
        {
            return true;
        }
    }
}

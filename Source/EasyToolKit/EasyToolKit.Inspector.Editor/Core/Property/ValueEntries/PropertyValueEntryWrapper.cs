using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a concrete implementation of a property value entry wrapper.
    /// This class wraps a base value entry to provide specialized type handling.
    /// </summary>
    /// <typeparam name="TValue">The specific type of the property value.</typeparam>
    /// <typeparam name="TBaseValue">The base type of the property value.</typeparam>
    public class PropertyValueEntryWrapper<TValue, TBaseValue> : IPropertyValueEntryWrapper<TValue, TBaseValue>
        where TValue : TBaseValue
    {
        private IPropertyValueEntry<TBaseValue> _valueEntry;
        private IPropertyValueCollectionWrapper<TValue, TBaseValue> _valuesWrapper;

        /// <summary>
        /// Initializes a new instance of the PropertyValueEntryWrapper class.
        /// </summary>
        /// <param name="valueEntry">The base value entry to wrap.</param>
        public PropertyValueEntryWrapper(IPropertyValueEntry<TBaseValue> valueEntry)
        {
            _valueEntry = valueEntry;
            _valuesWrapper = new PropertyValueCollectionWrapper<TValue, TBaseValue>(valueEntry.Values);
        }

        /// <summary>
        /// Gets or sets the strongly-typed property value.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        public TValue SmartValue
        {
            get => (TValue)_valueEntry.WeakSmartValue;
            set => _valueEntry.WeakSmartValue = value;
        }

        /// <summary>
        /// Gets the collection of strongly-typed property values for all targets.
        /// </summary>
        public IPropertyValueCollection<TValue> Values => _valuesWrapper;

        /// <summary>
        /// Gets or sets the property value as a weakly-typed object.
        /// This provides access to the property value without type safety.
        /// </summary>
        public object WeakSmartValue { get => _valueEntry.WeakSmartValue; set => _valueEntry.WeakSmartValue = value; }

        /// <summary>
        /// Gets the declared type of the property.
        /// This is the type as declared in the property definition.
        /// </summary>
        public Type BaseValueType => _valueEntry.BaseValueType;

        /// <summary>
        /// Gets the runtime type of the property value, if it can be determined.
        /// This may be null if the runtime type cannot be determined or varies between targets.
        /// </summary>
        public Type RuntimeValueType => _valueEntry.RuntimeValueType;

        /// <summary>
        /// Gets the collection of weakly-typed property values for all targets.
        /// </summary>
        public IPropertyValueCollection WeakValues => _valueEntry.WeakValues;

        /// <summary>
        /// Gets the inspector property associated with this value entry.
        /// </summary>
        public InspectorProperty Property => _valueEntry.Property;

        /// <summary>
        /// Gets the actual type of the property value.
        /// This may differ from BaseValueType if the runtime type is more specific.
        /// </summary>
        public Type ValueType => typeof(TValue);

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        public int ValueCount => _valueEntry.ValueCount;

        /// <summary>
        /// Occurs when a property value has changed.
        /// The parameter indicates the index of the target object whose value changed.
        /// </summary>
        public event Action<int> OnValueChanged
        {
            add => _valueEntry.OnValueChanged += value;
            remove => _valueEntry.OnValueChanged -= value;
        }

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        public bool ApplyChanges()
        {
            return _valueEntry.ApplyChanges();
        }

        /// <summary>
        /// Determines whether the property values are conflicted across different targets.
        /// A property is conflicted when different targets have different values for the same property.
        /// </summary>
        /// <returns>True if the property values are conflicted; otherwise, false.</returns>
        public bool IsConflicted()
        {
            return _valueEntry.IsConflicted();
        }

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        public void Update()
        {
            _valueEntry.Update();
        }

        /// <summary>
        /// Releases all resources used by this value entry wrapper.
        /// </summary>
        public void Dispose()
        {
            _valuesWrapper.Dispose();
        }
    }
}

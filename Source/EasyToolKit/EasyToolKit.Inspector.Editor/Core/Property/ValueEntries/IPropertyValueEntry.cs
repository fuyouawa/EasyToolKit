using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents an entry for a property value in the inspector.
    /// This interface provides access to property values and manages value changes.
    /// </summary>
    public interface IPropertyValueEntry : IDisposable
    {
        /// <summary>
        /// Gets the actual type of the property value.
        /// This may differ from BaseValueType if the runtime type is more specific.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets the declared type of the property.
        /// This is the type as declared in the property definition.
        /// </summary>
        Type BaseValueType { get; }

        /// <summary>
        /// Gets the runtime type of the property value, if it can be determined.
        /// This may be null if the runtime type cannot be determined or varies between targets.
        /// </summary>
        [CanBeNull] Type RuntimeValueType { get; }

        /// <summary>
        /// Gets the inspector property associated with this value entry.
        /// </summary>
        InspectorProperty Property { get; }

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        int ValueCount { get; }

        /// <summary>
        /// Gets or sets the property value as a weakly-typed object.
        /// This provides access to the property value without type safety.
        /// </summary>
        object WeakSmartValue { get; set; }

        /// <summary>
        /// Gets the collection of weakly-typed property values for all targets.
        /// </summary>
        IPropertyValueCollection WeakValues { get; }

        /// <summary>
        /// Occurs when a property value has changed.
        /// The parameter indicates the index of the target object whose value changed.
        /// </summary>
        event Action<int> OnValueChanged;

        /// <summary>
        /// Determines whether the property values are conflicted across different targets.
        /// A property is conflicted when different targets have different values for the same property.
        /// </summary>
        /// <returns>True if the property values are conflicted; otherwise, false.</returns>
        bool IsConflicted();

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        void Update();

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        bool ApplyChanges();
    }

    /// <summary>
    /// Represents a strongly-typed entry for a property value in the inspector.
    /// This generic interface provides type-safe access to property values.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public interface IPropertyValueEntry<TValue> : IPropertyValueEntry
    {
        /// <summary>
        /// Gets or sets the strongly-typed property value.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        TValue SmartValue { get; set; }

        /// <summary>
        /// Gets the collection of strongly-typed property values for all targets.
        /// </summary>
        IPropertyValueCollection<TValue> Values { get; }
    }
}
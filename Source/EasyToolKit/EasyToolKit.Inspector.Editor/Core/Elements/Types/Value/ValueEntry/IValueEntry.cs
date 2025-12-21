using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public delegate void ValueChangedHandler(int targetIndex);

    /// <summary>
    /// Represents an entry that manages the value of a property across multiple target objects.
    /// </summary>
    public interface IValueEntry
    {
        /// <summary>
        /// Gets the actual type of the property value.
        /// This may differ from <see cref="BaseValueType"/> if the runtime type is more specific.
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
        /// Gets the value element associated with this value entry.
        /// </summary>
        IValueElement Element { get; }

        /// <summary>
        /// Gets the state of the value entry.
        /// </summary>
        ValueEntryState State { get; }

        /// <summary>
        /// Gets the weakly‑typed collection of values across all targets.
        /// </summary>
        IValueCollection WeakValues { get; }

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
        /// Occurs when a property value has changed.
        /// The parameter indicates the index of the target object whose value changed.
        /// </summary>
        event ValueChangedHandler ValueChanged;

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

        /// <summary>
        /// Enqueues a change action to be applied later
        /// </summary>
        /// <param name="action">The action representing the change to be applied</param>
        void EnqueueChange(Action action);
    }

    /// <summary>
    /// Represents a strongly‑typed entry that manages the value of a property across multiple target objects.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public interface IValueEntry<TValue> : IValueEntry
    {
        /// <summary>
        /// Gets or sets the strongly-typed property value.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        TValue SmartValue { get; set; }

        IValueCollection<TValue> Values { get; }
    }
}

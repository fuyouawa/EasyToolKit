using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe wrapper for a value entry that exposes a more derived value type.
    /// Delegates all operations to the underlying <see cref="IValueEntry{TBaseValue}"/> while
    /// providing compile-time type safety for the more specific value type.
    /// </summary>
    /// <typeparam name="TValue">The derived value type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseValue">The base value type stored in the underlying value entry.</typeparam>
    public class ValueEntryWrapper<TValue, TBaseValue> : IValueEntryWrapper<TValue, TBaseValue>
        where TBaseValue : notnull
        where TValue : TBaseValue
    {
        private readonly IValueEntry<TBaseValue> _baseValueEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEntryWrapper{TValue, TBaseValue}"/> class.
        /// </summary>
        /// <param name="baseValueEntry">The underlying value entry to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when baseValueEntry is null.</exception>
        public ValueEntryWrapper([NotNull] IValueEntry<TBaseValue> baseValueEntry)
        {
            _baseValueEntry = baseValueEntry ?? throw new ArgumentNullException(nameof(baseValueEntry));
        }

        /// <summary>
        /// Occurs before a value is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> BeforeValueChanged
        {
            add => _baseValueEntry.BeforeValueChanged += value;
            remove => _baseValueEntry.BeforeValueChanged -= value;
        }

        /// <summary>
        /// Occurs after a value has been changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> AfterValueChanged
        {
            add => _baseValueEntry.AfterValueChanged += value;
            remove => _baseValueEntry.AfterValueChanged -= value;
        }

        /// <summary>
        /// Gets the value element that owns this value entry.
        /// </summary>
        public IValueElement OwnerElement => _baseValueEntry.OwnerElement;

        /// <summary>
        /// Gets a value indicating whether the value is read-only.
        /// </summary>
        public bool IsReadOnly => _baseValueEntry.IsReadOnly;

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        public int TargetCount => _baseValueEntry.TargetCount;

        /// <summary>
        /// Gets the declared type of the value.
        /// </summary>
        public Type ValueType => typeof(TValue);

        public Type RuntimeValueType => _baseValueEntry.RuntimeValueType;

        /// <summary>
        /// Gets or sets the value as a weakly-typed object.
        /// </summary>
        public object WeakSmartValue
        {
            get => _baseValueEntry.WeakSmartValue;
            set => _baseValueEntry.WeakSmartValue = value;
        }

        /// <summary>
        /// Gets or sets the strongly-typed value.
        /// </summary>
        public TValue SmartValue
        {
            get => (TValue)_baseValueEntry.SmartValue;
            set => _baseValueEntry.SmartValue = value;
        }

        /// <summary>
        /// Gets a value indicating whether any values have been modified.
        /// </summary>
        public bool IsDirty => _baseValueEntry.IsDirty;

        /// <summary>
        /// Gets the current state of the value entry.
        /// </summary>
        public ValueEntryState State => _baseValueEntry.State;

        /// <summary>
        /// Gets the underlying value entry.
        /// </summary>
        public IValueEntry<TBaseValue> BaseValueEntry => _baseValueEntry;

        /// <summary>
        /// Gets the weakly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <returns>The value for the specified target.</returns>
        public object GetWeakValue(int targetIndex)
        {
            return _baseValueEntry.GetWeakValue(targetIndex);
        }

        /// <summary>
        /// Sets the weakly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The value to set.</param>
        public void SetWeakValue(int targetIndex, object value)
        {
            _baseValueEntry.SetWeakValue(targetIndex, value);
        }

        /// <summary>
        /// Gets the strongly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <returns>The value for the specified target.</returns>
        public TValue GetValue(int targetIndex)
        {
            return (TValue)_baseValueEntry.GetValue(targetIndex);
        }

        /// <summary>
        /// Sets the strongly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(int targetIndex, TValue value)
        {
            _baseValueEntry.SetValue(targetIndex, value);
        }

        /// <summary>
        /// Marks the value entry as dirty, indicating that values have been modified.
        /// </summary>
        public void MarkDirty()
        {
            _baseValueEntry.MarkDirty();
        }

        /// <summary>
        /// Clears the dirty flag, indicating that no values have been modified.
        /// </summary>
        public void ClearDirty()
        {
            _baseValueEntry.ClearDirty();
        }

        /// <summary>
        /// Enqueues a change action to be applied during <see cref="ApplyChanges"/>.
        /// </summary>
        /// <param name="action">The action representing the change to be applied.</param>
        public void EnqueueChange(Action action)
        {
            _baseValueEntry.EnqueueChange(action);
        }

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        public void ApplyChanges()
        {
            _baseValueEntry.ApplyChanges();
        }

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// </summary>
        public void Update()
        {
            _baseValueEntry.Update();
        }
    }
}

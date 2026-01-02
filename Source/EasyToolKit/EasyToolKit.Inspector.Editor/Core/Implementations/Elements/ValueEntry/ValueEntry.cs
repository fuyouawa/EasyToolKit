using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents a value entry that manages property values for multiple target objects.
    /// This class combines the functionality of the old PropertyValueEntry and PropertyValueCollection,
    /// providing value storage, change tracking, and change notification.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public class ValueEntry<TValue> : IValueEntry<TValue>
    {
        private static readonly bool IsInstantiableType = typeof(TValue).IsInstantiable();
        private static readonly bool IsStructuralType = typeof(TValue).IsStructuralType();

        private readonly Type[] _runtimeValueTypes;
        private readonly TValue[] _values;
        private IValueOperation<TValue> _operation;
        private readonly Queue<Action> _pendingChanges;

        private int? _lastUpdateId;
        private ValueEntryState? _cachedState;
        private readonly IValueElement _ownerElement;

        /// <summary>
        /// Occurs before a value is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> BeforeValueChanged;

        /// <summary>
        /// Occurs after a value has been changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> AfterValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEntry{TValue}"/> class.
        /// </summary>
        /// <param name="ownerElement">The value element that owns this value entry.</param>
        public ValueEntry([NotNull] IValueElement ownerElement)
        {
            _ownerElement = ownerElement ?? throw new ArgumentNullException(nameof(ownerElement));

            _values = new TValue[_ownerElement.SharedContext.Tree.Targets.Count];
            _runtimeValueTypes = new Type[_ownerElement.SharedContext.Tree.Targets.Count];
            _pendingChanges = new Queue<Action>();
        }

        /// <summary>
        /// Gets the value element that owns this value entry.
        /// </summary>
        public IValueElement OwnerElement => _ownerElement;

        /// <summary>
        /// Gets a value indicating whether the value is read-only.
        /// </summary>
        public bool IsReadOnly => Operation.IsReadOnly;

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        public int TargetCount => _values.Length;

        /// <summary>
        /// Gets the declared type of the value.
        /// This is the type as declared in the value definition.
        /// </summary>
        public Type ValueType => typeof(TValue);

        public Type RuntimeValueType
        {
            get
            {
                if (State == ValueEntryState.Consistent || State == ValueEntryState.TypeConsistent)
                {
                    var type = _runtimeValueTypes[0];
                    if (type == null)
                    {
                        if (typeof(TValue).IsValueType || typeof(TValue).IsBasicValueType())
                        {
                            return typeof(TValue);
                        }
                    }

                    return type;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the value as a weakly-typed object.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        public object WeakSmartValue
        {
            get => SmartValue;
            set => SmartValue = (TValue)value;
        }

        /// <summary>
        /// Gets the value operation for this entry.
        /// </summary>
        protected IValueOperation<TValue> Operation
        {
            get
            {
                if (_operation == null)
                {
                    var factory = _ownerElement.SharedContext.GetResolverFactory<IValueOperationResolver>();
                    var resolver = factory.CreateResolver(_ownerElement);
                    if (resolver == null)
                    {
                        throw new InvalidOperationException($"Can not create value operation resolver for value entry of '{_ownerElement}'.");
                    }

                    resolver.Element = _ownerElement;

                    _operation = (IValueOperation<TValue>)resolver.GetOperation();
                }
                return _operation;
            }
        }

        /// <summary>
        /// Gets or sets the strongly-typed value.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        public TValue SmartValue
        {
            get => GetValue(0);
            set
            {
                for (int i = 0; i < _values.Length; i++)
                {
                    SetValue(i, value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether any values have been modified.
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the current state of the value entry.
        /// </summary>
        public ValueEntryState State
        {
            get
            {
                if (_cachedState.HasValue)
                {
                    return _cachedState.Value;
                }

                _cachedState = CalculateState();
                return _cachedState.Value;
            }
        }

        /// <summary>
        /// Gets the weakly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <returns>The value for the specified target.</returns>
        public object GetWeakValue(int targetIndex)
        {
            return GetValue(targetIndex);
        }

        /// <summary>
        /// Sets the weakly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The value to set.</param>
        public void SetWeakValue(int targetIndex, object value)
        {
            SetValue(targetIndex, (TValue)value);
        }

        /// <summary>
        /// Gets the strongly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <returns>The value for the specified target.</returns>
        public TValue GetValue(int targetIndex)
        {
            return _values[targetIndex];
        }

        /// <summary>
        /// Sets the strongly-typed value at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(int targetIndex, TValue value)
        {
            if (IsReadOnly)
            {
                Debug.LogWarning($"Value '{_ownerElement.Path}' cannot be edited.");
                return;
            }

            var oldValue = _values[targetIndex];
            if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
            {
                return;
            }

            using (var eventArgs = ValueChangedEventArgs.Create(targetIndex, oldValue, value, ValueChangedTiming.Before))
            {
                BeforeValueChanged?.Invoke(_ownerElement, eventArgs);
            }

            _values[targetIndex] = value;
            MarkDirty();

            using (var eventArgs = ValueChangedEventArgs.Create(targetIndex, oldValue, value, ValueChangedTiming.After))
            {
                AfterValueChanged?.Invoke(_ownerElement, eventArgs);
            }
        }

        /// <summary>
        /// Marks the value entry as dirty, indicating that values have been modified.
        /// </summary>
        public void MarkDirty()
        {
            if (!IsDirty)
            {
                IsDirty = true;
                _cachedState = null;
                using var eventArgs = ValueDirtyEventArgs.Create();
                OwnerElement.SharedContext.TriggerEvent(_ownerElement, eventArgs);
            }
        }

        /// <summary>
        /// Clears the dirty flag, indicating that no values have been modified.
        /// </summary>
        public void ClearDirty()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Enqueues a change action to be applied during <see cref="ApplyChanges"/>.
        /// </summary>
        /// <param name="action">The action representing the change to be applied.</param>
        public void EnqueueChange(Action action)
        {
            _pendingChanges.Enqueue(action);
            MarkDirty();
        }

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        public void ApplyChanges()
        {
            if (!IsDirty)
            {
                return;
            }

            var tree = _ownerElement.SharedContext.Tree;
            var targets = tree.Targets;

            // Record undo for Unity objects
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] is UnityEngine.Object unityObj)
                {
                    Undo.RecordObject(unityObj, $"Change {_ownerElement.Path}");
                }
            }

            while (_pendingChanges.Count > 0)
            {
                var change = _pendingChanges.Dequeue();
                change?.Invoke();
            }

            // Apply value changes
            for (int i = 0; i < _values.Length; i++)
            {
                var owner = GetOwner(i);
                if (owner == null)
                {
                    continue;
                }

                Operation.SetValue(ref owner, _values[i]);
                SetOwner(i, owner);
            }

            ClearDirty();
            _cachedState = null;
        }

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        public void Update()
        {
            if (_lastUpdateId == _ownerElement.SharedContext.UpdateId)
            {
                return;
            }

            _lastUpdateId = _ownerElement.SharedContext.UpdateId;
            _cachedState = null;

            var tree = _ownerElement.SharedContext.Tree;
            bool clearDirty = true;

            for (int i = 0; i < tree.Targets.Count; i++)
            {
                var owner = GetOwner(i);

                TValue value;
                Type runtimeValueType;
                if (owner == null)
                {
                    value = _values[i];
                    runtimeValueType = value?.GetType() ?? typeof(TValue);
                }
                else
                {
                    value = Operation.GetValue(ref owner);
                    runtimeValueType = Operation.GetValueRuntimeType(ref owner);

                    // Auto-instantiate null values for instantiable types
                    if (value == null && IsInstantiableType)
                    {
                        if (typeof(TValue).TryCreateInstance(out value))
                        {
                            runtimeValueType = typeof(TValue);
                            MarkDirty();
                            clearDirty = false;
                        }
                    }
                }

                _values[i] = value;
                _runtimeValueTypes[i] = runtimeValueType;
            }

            if (clearDirty)
            {
                ClearDirty();
            }
        }

        /// <summary>
        /// Calculates the current state of the value entry.
        /// </summary>
        /// <returns>The current value entry state.</returns>
        private ValueEntryState CalculateState()
        {
            if (_values.Length <= 1)
            {
                return ValueEntryState.Consistent;
            }

            var first = _values[0];
            var firstRuntimeType = _runtimeValueTypes[0];
            for (int i = 1; i < _values.Length; i++)
            {
                if (IsStructuralType)
                {
                    if (firstRuntimeType != _runtimeValueTypes[i])
                    {
                        return ValueEntryState.Mixed;
                    }
                }
                else
                {
                    if (!EqualityComparer<TValue>.Default.Equals(first, _values[i]))
                    {
                        return ValueEntryState.Mixed;
                    }
                }
            }

            if (IsStructuralType)
            {
                for (int i = 1; i < _values.Length; i++)
                {
                    if (!EqualityComparer<TValue>.Default.Equals(first, _values[i]))
                    {
                        return ValueEntryState.TypeConsistent;
                    }
                }
            }

            return ValueEntryState.Consistent;
        }

        /// <summary>
        /// Gets the owner object for the specified target index.
        /// </summary>
        /// <param name="targetIndex">The target index.</param>
        /// <returns>The owner object.</returns>
        [CanBeNull]
        private object GetOwner(int targetIndex)
        {
            // For root elements, the owner is the target index
            if (_ownerElement.Definition.Roles.IsRoot())
            {
                return targetIndex;
            }

            if (_ownerElement.LogicalParent is IValueElement valueParentElement)
            {
                // For child elements, get the owner from the parent's value entry
                return valueParentElement.ValueEntry.GetWeakValue(targetIndex);
            }

            return null;
        }

        /// <summary>
        /// Sets the owner object for the specified target index.
        /// </summary>
        /// <param name="targetIndex">The target index.</param>
        /// <param name="owner">The owner object.</param>
        private void SetOwner(int targetIndex, object owner)
        {
            // For child elements, update the owner in the parent's value entry
            if (_ownerElement.Definition is IRootDefinition)
            {
                return; // Root elements don't need to update the target
            }

            if (_ownerElement.LogicalParent is IValueElement valueParentElement)
            {
                valueParentElement.ValueEntry.SetWeakValue(targetIndex, owner);
            }
        }
    }
}

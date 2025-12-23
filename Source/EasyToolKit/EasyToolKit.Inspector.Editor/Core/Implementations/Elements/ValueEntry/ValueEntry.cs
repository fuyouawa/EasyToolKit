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

        private readonly TValue[] _values;
        private readonly IValueOperation<TValue> _operation;
        private readonly List<Action> _queuedChanges = new List<Action>();

        private int? _lastUpdateId;
        private ValueEntryState? _cachedState;

        /// <summary>
        /// Gets the value element that owns this value entry.
        /// </summary>
        public IValueElement OwnerElement { get; }

        /// <summary>
        /// Gets a value indicating whether the value is read-only.
        /// </summary>
        public bool IsReadOnly => _operation.IsReadOnly;

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        public int TargetCount => _values.Length;

        /// <summary>
        /// Gets the declared type of the value.
        /// This is the type as declared in the value definition.
        /// </summary>
        public Type ValueType => typeof(TValue);

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
        /// Occurs before a value is changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> PreValueChanged;

        /// <summary>
        /// Occurs after a value has been changed.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> PostValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEntry{TValue}"/> class.
        /// </summary>
        /// <param name="ownerElement">The value element that owns this value entry.</param>
        public ValueEntry([NotNull] IValueElement ownerElement)
        {
            OwnerElement = ownerElement ?? throw new ArgumentNullException(nameof(ownerElement));

            var factory = OwnerElement.SharedContext.GetResolverFactory<IValueOperationResolver>();
            var resolver = factory.CreateResolver(OwnerElement);
            _operation = (IValueOperation<TValue>)resolver.GetOperation();

            _values = new TValue[OwnerElement.SharedContext.Tree.Targets.Count];
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
                Debug.LogWarning($"Value '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var oldValue = _values[targetIndex];
            if (EqualityComparer<TValue>.Default.Equals(oldValue, value))
            {
                return;
            }

            OnPreValueChanged(targetIndex, oldValue, value);

            _values[targetIndex] = value;
            MarkDirty();

            OnPostValueChanged(targetIndex, oldValue, value);
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
            _queuedChanges.Add(action);
            MarkDirty();
        }

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        public void ApplyChanges()
        {
            if (!IsDirty && _queuedChanges.Count == 0)
            {
                return;
            }

            var tree = OwnerElement.SharedContext.Tree;
            var targets = tree.Targets;

            // Record undo for Unity objects
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] is UnityEngine.Object unityObj)
                {
                    Undo.RecordObject(unityObj, $"Change {OwnerElement.Path}");
                }
            }

            // Apply queued changes
            foreach (var change in _queuedChanges)
            {
                change();
            }
            _queuedChanges.Clear();

            // Apply value changes
            for (int i = 0; i < _values.Length; i++)
            {
                var owner = GetOwner(i);
                if (owner == null)
                {
                    continue;
                }

                _operation.SetValue(ref owner, _values[i]);
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
            if (_lastUpdateId == OwnerElement.SharedContext.UpdateId)
            {
                return;
            }

            _lastUpdateId = OwnerElement.SharedContext.UpdateId;
            _cachedState = null;

            var tree = OwnerElement.SharedContext.Tree;
            bool clearDirty = true;

            for (int i = 0; i < tree.Targets.Count; i++)
            {
                var owner = GetOwner(i);
                if (owner == null)
                {
                    _values[i] = default;
                    continue;
                }

                var value = _operation.GetValue(ref owner);

                // Auto-instantiate null values for instantiable types
                if (value == null && IsInstantiableType)
                {
                    if (typeof(TValue).TryCreateInstance(out _values[i]))
                    {
                        MarkDirty();
                        clearDirty = false;
                        continue;
                    }
                }

                _values[i] = value;
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
            for (int i = 1; i < _values.Length; i++)
            {
                if (!EqualityComparer<TValue>.Default.Equals(first, _values[i]))
                {
                    return ValueEntryState.Mixed;
                }
            }

            return ValueEntryState.Consistent;
        }

        /// <summary>
        /// Raises the <see cref="PreValueChanged"/> event.
        /// </summary>
        /// <param name="targetIndex">The index of the target object whose value is changing.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPreValueChanged(int targetIndex, TValue oldValue, TValue newValue)
        {
            PreValueChanged?.Invoke(this, new ValueChangedEventArgs(targetIndex, oldValue, newValue));
        }

        /// <summary>
        /// Raises the <see cref="PostValueChanged"/> event.
        /// </summary>
        /// <param name="targetIndex">The index of the target object whose value changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnPostValueChanged(int targetIndex, TValue oldValue, TValue newValue)
        {
            var args = new ValueChangedEventArgs(targetIndex, oldValue, newValue);
            PostValueChanged?.Invoke(this, args);

            // Queue callback until repaint for proper GUI timing
            OwnerElement.SharedContext.Tree.QueueCallbackUntilRepaint(() =>
            {
                try
                {
                    PostValueChanged?.Invoke(this, args);
                }
                catch (ExitGUIException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }

        /// <summary>
        /// Gets the owner object for the specified target index.
        /// </summary>
        /// <param name="targetIndex">The target index.</param>
        /// <returns>The owner object.</returns>
        private object GetOwner(int targetIndex)
        {
            // For root elements, the owner is the target itself
            if (OwnerElement.Definition is IRootDefinition)
            {
                return OwnerElement.SharedContext.Tree.Targets[targetIndex];
            }

            // For child elements, get the owner from the parent's value entry
            return OwnerElement.LogicalParent?.ValueEntry.GetWeakValue(targetIndex);
        }

        /// <summary>
        /// Sets the owner object for the specified target index.
        /// </summary>
        /// <param name="targetIndex">The target index.</param>
        /// <param name="owner">The owner object.</param>
        private void SetOwner(int targetIndex, object owner)
        {
            // For child elements, update the owner in the parent's value entry
            if (OwnerElement.Definition is IRootDefinition)
            {
                return; // Root elements don't need to update the target
            }

            OwnerElement.LogicalParent?.ValueEntry.SetWeakValue(targetIndex, owner);
        }

        /// <summary>
        /// Releases all resources used by this value entry.
        /// </summary>
        public void Dispose()
        {
            _queuedChanges.Clear();
        }
    }
}

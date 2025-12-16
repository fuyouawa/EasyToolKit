using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a concrete implementation of a property value entry for strongly-typed values.
    /// This class manages property values for multiple target objects and handles value synchronization.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public class PropertyValueEntry<TValue> : IPropertyValueEntry<TValue>
    {
        private bool? _isConflictedCache;
        private int? _lastUpdateId;
        private Type _runtimeValueType;

        /// <summary>
        /// Gets the inspector property associated with this value entry.
        /// </summary>
        public InspectorProperty Property { get; private set; }

        /// <summary>
        /// Gets the collection of strongly-typed property values for all targets.
        /// </summary>
        public IPropertyValueCollection<TValue> Values { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueEntry{TValue}"/> class.
        /// </summary>
        /// <param name="property">The inspector property associated with this value entry.</param>
        public PropertyValueEntry(InspectorProperty property)
        {
            Property = property;
            Values = new PropertyValueCollection<TValue>(property);
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// The parameter indicates the index of the target object whose value changed.
        /// </summary>
        public event Action<int> OnValueChanged;

        /// <summary>
        /// Gets or sets the property value as a weakly-typed object.
        /// This provides access to the property value without type safety.
        /// </summary>
        public object WeakSmartValue
        {
            get => SmartValue;
            set => SmartValue = (TValue)value;
        }

        /// <summary>
        /// Gets or sets the strongly-typed property value.
        /// Setting this value will apply the same value to all targets.
        /// </summary>
        public TValue SmartValue
        {
            get => Values[0];
            set
            {
                for (int i = 0; i < Values.Count; i++)
                {
                    Values[i] = value;
                }
            }
        }

        /// <summary>
        /// Gets the declared type of the property.
        /// This is the type as declared in the property definition.
        /// </summary>
        public Type BaseValueType => typeof(TValue);

        /// <summary>
        /// Gets the collection of weakly-typed property values for all targets.
        /// </summary>
        public IPropertyValueCollection WeakValues => Values;

        /// <summary>
        /// Gets the runtime type of the property value, if it can be determined.
        /// This may be null if the runtime type cannot be determined or varies between targets.
        /// </summary>
        [CanBeNull] public Type RuntimeValueType => _runtimeValueType;

        /// <summary>
        /// Gets the actual type of the property value.
        /// This may differ from <see cref="BaseValueType"/> if the runtime type is more specific.
        /// </summary>
        public Type ValueType
        {
            get
            {
                if (RuntimeValueType != null)
                {
                    return RuntimeValueType;
                }

                return BaseValueType;
            }
        }

        /// <summary>
        /// Gets the number of target objects that this value entry manages.
        /// </summary>
        public int ValueCount => Values.Count;

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        public void Update()
        {
            if (_lastUpdateId == Property.Tree.UpdateId)
            {
                return;
            }
            _lastUpdateId = Property.Tree.UpdateId;

            _isConflictedCache = null;
            Values.Update();

            TryGetRuntimeValueType(out _runtimeValueType);
        }

        /// <summary>
        /// Applies any pending changes from this value entry back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        public bool ApplyChanges()
        {
            bool changed = false;
            if (Values.Dirty)
            {
                if (Property.Tree.Targets[0] is UnityEngine.Object)
                {
                    foreach (UnityEngine.Object target in Property.Tree.Targets)
                    {
                        Undo.RecordObject(target, $"Change {Property.Path} on {target.name}");
                    }
                }

                changed = Values.ApplyChanges();

                if (changed)
                {
                    for (int i = 0; i < Property.Tree.Targets.Count; i++)
                    {
                        TriggerValueChanged(i);
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// Determines whether the property values are conflicted across different targets.
        /// A property is conflicted when different targets have different values for the same property.
        /// </summary>
        /// <returns>True if the property values are conflicted; otherwise, false.</returns>
        public bool IsConflicted()
        {
            if (_isConflictedCache.HasValue)
            {
                return _isConflictedCache.Value;
            }

            _isConflictedCache = IsConflictedImpl();
            return _isConflictedCache.Value;
        }

        /// <summary>
        /// Implementation of the conflict detection logic.
        /// </summary>
        /// <returns>True if the property values are conflicted; otherwise, false.</returns>
        private bool IsConflictedImpl()
        {
            if (Property.Info.IsUnityProperty)
            {
                var serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.UnityPath);
                return serializedProperty.hasMultipleDifferentValues;
            }

            if (Values.Count > 1)
            {
                var first = Values[0];
                for (int i = 1; i < Values.Count; i++)
                {
                    if (!EqualityComparer<TValue>.Default.Equals(first, Values[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to determine the runtime type of the property values.
        /// </summary>
        /// <param name="runtimeValueType">The determined runtime type, or null if it cannot be determined.</param>
        /// <returns>True if a consistent runtime type was determined; otherwise, false.</returns>
        private bool TryGetRuntimeValueType(out Type runtimeValueType)
        {
            if (BaseValueType.IsValueType || BaseValueType.IsSealed)
            {
                runtimeValueType = BaseValueType;
                return true;
            }

            runtimeValueType = null;
            for (int i = 0; i < Values.Count; i++)
            {
                object value = Values[i];
                if (value == null)
                {
                    return false;
                }

                var valueType = value.GetType();

                if (runtimeValueType == null)
                {
                    runtimeValueType = valueType;
                }
                else if (runtimeValueType != valueType)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Triggers the value changed event for the specified target index.
        /// This method ensures the event is triggered at the appropriate time in the GUI cycle.
        /// </summary>
        /// <param name="index">The index of the target object whose value changed.</param>
        internal void TriggerValueChanged(int index)
        {
            void Action()
            {
                if (this.OnValueChanged != null)
                {
                    try
                    {
                        this.OnValueChanged(index);
                    }
                    catch (ExitGUIException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                Property.Tree.InvokePropertyValueChanged(Property, index);
            }


            if (Event.current != null && Event.current.type == EventType.Repaint)
            {
                Action();
            }
            else
            {
                Property.Tree.QueueCallbackUntilRepaint(Action);
            }
        }

        /// <summary>
        /// Releases all resources used by this value entry.
        /// </summary>
        public void Dispose()
        {
            Values.Dispose();
            Property = null;
            Values = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a concrete implementation of a property value collection for strongly-typed values.
    /// This class manages property values for multiple target objects and handles value synchronization.
    /// </summary>
    /// <typeparam name="TValue">The type of the property values.</typeparam>
    public sealed class PropertyValueCollection<TValue> : IPropertyValueCollection<TValue>
    {
        /// <summary>
        /// Indicates whether the value type can be instantiated (i.e., has a parameterless constructor).
        /// </summary>
        public static readonly bool IsInstantiableType = typeof(TValue).IsInstantiable();

        /// <summary>
        /// Gets the inspector property associated with this value collection.
        /// </summary>
        public InspectorProperty Property { get; private set; }

        private TValue[] _values;
        private bool _firstUpdated = false;

        /// <summary>
        /// Gets a value indicating whether any values in this collection have been modified.
        /// </summary>
        public bool Dirty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueCollection{TValue}"/> class.
        /// </summary>
        /// <param name="property">The inspector property associated with this value collection.</param>
        public PropertyValueCollection(InspectorProperty property)
        {
            Property = property;
            _values = new TValue[property.Tree.Targets.Count];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of property values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return ((IEnumerable<TValue>)_values).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of property values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of target objects in this collection.
        /// </summary>
        public int Count => _values.Length;

        /// <summary>
        /// Gets or sets the property value at the specified target index as a weakly-typed object.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The property value for the specified target.</returns>
        object IPropertyValueCollection.this[int index]
        {
            get => this[index];
            set => this[index] = (TValue)value;
        }

        /// <summary>
        /// Gets or sets the strongly-typed property value at the specified target index.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The strongly-typed property value for the specified target.</returns>
        public TValue this[int index]
        {
            get => _values[index];
            set
            {
                if (!ReferenceEquals(_values[index], value) && !EqualityComparer<TValue>.Default.Equals(_values[index], value))
                {
                    if (Property.IsReadOnly)
                    {
                        Debug.LogWarning($"Property '{Property.Path}' cannot be edited.");
                        return;
                    }

                    _values[index] = value;
                    MakeDirty();
                }
            }
        }

        /// <summary>
        /// Marks the collection as dirty, indicating that values have been modified.
        /// </summary>
        private void MakeDirty()
        {
            if (!Dirty)
            {
                Dirty = true;
                Property.Tree.SetPropertyDirty(Property);
            }
        }

        /// <summary>
        /// Clears the dirty flag, indicating that no values have been modified.
        /// </summary>
        private void ClearDirty()
        {
            Dirty = false;
        }

        /// <summary>
        /// Forces the collection to be marked as dirty, indicating that values have been modified.
        /// </summary>
        public void ForceMakeDirty()
        {
            MakeDirty();
        }

        /// <summary>
        /// Updates the collection with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        public void Update()
        {
            bool clearDirty = true;
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                var owner = Property.Info.IsLogicRoot
                    ? i
                    : Property.Parent.ValueEntry.WeakValues[i];

                if (owner == null)
                {
                    _values[i] = default;
                    continue;
                }

                var value = (TValue)Property.GetOperation().GetWeakValue(ref owner);
                if (value == null && IsInstantiableType)
                {
                    if (typeof(TValue).TryCreateInstance(out _values[i]))
                    {
                        MakeDirty();
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

            _firstUpdated = true;
        }

        /// <summary>
        /// Applies any pending changes from this collection back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        public bool ApplyChanges()
        {
            if (!Dirty) return false;

            Assert.IsTrue(Property.Parent?.ValueEntry != null);

            for (int i = 0; i < _values.Length; i++)
            {
                var owner = Property.Parent.ValueEntry.WeakValues[i];
                var value = _values[i];
                Property.GetOperation().SetWeakValue(ref owner, value);
                Property.Parent.ValueEntry.WeakValues[i] = owner;
            }

            ClearDirty();
            return true;
        }

        /// <summary>
        /// Releases all resources used by this value collection.
        /// </summary>
        public void Dispose()
        {
            Property = null;
            _values = null;
        }
    }
}

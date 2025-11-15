using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a concrete implementation of a property value collection wrapper.
    /// This class wraps a base value collection to provide specialized type handling.
    /// </summary>
    /// <typeparam name="TValue">The specific type of the property values.</typeparam>
    /// <typeparam name="TBaseValue">The base type of the property values.</typeparam>
    public class PropertyValueCollectionWrapper<TValue, TBaseValue> : IPropertyValueCollectionWrapper<TValue, TBaseValue>
        where TValue : TBaseValue
    {
        private readonly IPropertyValueCollection<TBaseValue> _collection;

        /// <summary>
        /// Initializes a new instance of the PropertyValueCollectionWrapper class.
        /// </summary>
        /// <param name="collection">The base value collection to wrap.</param>
        public PropertyValueCollectionWrapper(IPropertyValueCollection<TBaseValue> collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Gets or sets the strongly-typed property value at the specified target index.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The strongly-typed property value for the specified target.</returns>
        public TValue this[int index]
        {
            get => (TValue)_collection[index];
            set => _collection[index] = value;
        }

        /// <summary>
        /// Gets or sets the property value at the specified target index as a weakly-typed object.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The property value for the specified target.</returns>
        object IPropertyValueCollection.this[int index]
        {
            get => (_collection as IPropertyValueCollection)[index];
            set => (_collection as IPropertyValueCollection)[index] = value;
        }

        /// <summary>
        /// Gets the number of target objects in this collection.
        /// </summary>
        public int Count => _collection.Count;

        /// <summary>
        /// Gets the inspector property associated with this value collection.
        /// </summary>
        public InspectorProperty Property => _collection.Property;

        /// <summary>
        /// Gets a value indicating whether any values in this collection have been modified.
        /// </summary>
        public bool Dirty => _collection.Dirty;

        /// <summary>
        /// Applies any pending changes from this collection back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        public bool ApplyChanges()
        {
            return _collection.ApplyChanges();
        }

        /// <summary>
        /// Forces the collection to be marked as dirty, indicating that values have been modified.
        /// </summary>
        public void ForceMakeDirty()
        {
            _collection.ForceMakeDirty();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of property values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TBaseValue item in _collection)
            {
                yield return (TValue)item;
            }
        }

        /// <summary>
        /// Updates the collection with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        public void Update()
        {
            _collection.Update();
        }

        /// <summary>
        /// Releases all resources used by this value collection wrapper.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of property values.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
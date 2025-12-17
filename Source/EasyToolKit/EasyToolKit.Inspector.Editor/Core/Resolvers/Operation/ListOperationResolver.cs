using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Operation resolver for IList{T} collections in the inspector system.
    /// Handles list-specific operations like indexed access, insertion, and removal.
    /// Implements IOrderedCollectionOperationResolver for position-based operations.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement IList{TElement})</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class ListOperationResolver<TCollection, TElement> : OrderedCollectionOperationResolver<TCollection, TElement>
        where TCollection : IList<TElement>
    {
        private readonly IPropertyValueEntry<TCollection> _valueEntry;

        /// <summary>
        /// Initializes a new instance of the ListOperationResolver
        /// </summary>
        /// <param name="valueEntry">The property value entry for the collection</param>
        public ListOperationResolver(IPropertyValueEntry<TCollection> valueEntry)
        {
            _valueEntry = valueEntry ?? throw new ArgumentNullException(nameof(valueEntry));
        }

        /// <summary>
        /// Gets whether the collection is read-only
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                // Check if any of the collection instances are read-only
                for (int i = 0; i < _valueEntry.Values.Count; i++)
                {
                    if (_valueEntry.Values[i].IsReadOnly)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        public override Type CollectionType => typeof(TCollection);

        /// <summary>
        /// Actually adds an element to the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to add to the collection</param>
        protected override void AddElement(int targetIndex, TElement value)
        {
            if (targetIndex < 0 || targetIndex >= _valueEntry.Values.Count)
                throw new ArgumentOutOfRangeException(nameof(targetIndex));

            var collection = _valueEntry.Values[targetIndex];
            collection?.Add(value);
        }

        /// <summary>
        /// Actually removes an element from the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to remove from the collection</param>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            if (targetIndex < 0 || targetIndex >= _valueEntry.Values.Count)
                throw new ArgumentOutOfRangeException(nameof(targetIndex));

            var collection = _valueEntry.Values[targetIndex];
            collection?.Remove(value);
        }

        /// <summary>
        /// Actually inserts an element into the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            if (targetIndex < 0 || targetIndex >= _valueEntry.Values.Count)
                return;

            var collection = _valueEntry.Values[targetIndex];
            if (collection != null)
            {
                if (index >= 0 && index <= collection.Count)
                {
                    collection.Insert(index, value);
                }
                else
                {
                    // If index is out of bounds, add to the end
                    collection.Add(value);
                }
            }
        }

        /// <summary>
        /// Actually removes an element from the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        protected override void RemoveElementAt(int targetIndex, int index)
        {
            if (targetIndex < 0 || targetIndex >= _valueEntry.Values.Count)
                return;

            var collection = _valueEntry.Values[targetIndex];
            if (collection != null && index >= 0 && index < collection.Count)
            {
                collection.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets an element at the specified index from a collection
        /// </summary>
        /// <param name="collection">The collection to get the element from</param>
        /// <param name="elementIndex">The index of the element</param>
        /// <returns>The element at the specified index</returns>
        protected override TElement GetElementAt(TCollection collection, int elementIndex)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (elementIndex < 0 || elementIndex >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            return collection[elementIndex];
        }

        /// <summary>
        /// Sets an element at the specified index in a collection
        /// </summary>
        /// <param name="collection">The collection to set the element in</param>
        /// <param name="elementIndex">The index of the element to set</param>
        /// <param name="value">The value to set</param>
        protected override void SetElementAt(TCollection collection, int elementIndex, TElement value)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (elementIndex < 0 || elementIndex >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            collection[elementIndex] = value;
        }
    }
}
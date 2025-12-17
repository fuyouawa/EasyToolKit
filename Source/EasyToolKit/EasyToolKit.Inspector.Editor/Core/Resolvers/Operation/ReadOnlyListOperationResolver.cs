using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Operation resolver for IReadOnlyList{T} collections in the inspector system.
    /// Provides read-only access to collection elements and prevents modifications.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement IReadOnlyList{TElement})</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class ReadOnlyListOperationResolver<TCollection, TElement> : CollectionOperationResolver<TCollection, TElement>
        where TCollection : IReadOnlyList<TElement>
    {
        private readonly IPropertyValueEntry<TCollection> _valueEntry;

        /// <summary>
        /// Initializes a new instance of the ReadOnlyListOperationResolver
        /// </summary>
        /// <param name="valueEntry">The property value entry for the collection</param>
        public ReadOnlyListOperationResolver(IPropertyValueEntry<TCollection> valueEntry)
        {
            _valueEntry = valueEntry ?? throw new ArgumentNullException(nameof(valueEntry));
        }

        /// <summary>
        /// Gets whether the collection is read-only (always returns true)
        /// </summary>
        public override bool IsReadOnly => true;

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        public override Type CollectionType => typeof(TCollection);

        /// <summary>
        /// Attempts to add an element to the collection. Always throws NotSupportedException.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to add to the collection</param>
        /// <exception cref="NotSupportedException">Always thrown as read-only lists cannot be modified</exception>
        protected override void AddElement(int targetIndex, TElement value)
        {
            throw new NotSupportedException("Readonly list cannot be modified");
        }

        /// <summary>
        /// Attempts to remove an element from the collection. Always throws NotSupportedException.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to remove from the collection</param>
        /// <exception cref="NotSupportedException">Always thrown as read-only lists cannot be modified</exception>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            throw new NotSupportedException("Readonly list cannot be modified");
        }

        /// <summary>
        /// Gets the element count for a read-only list
        /// </summary>
        /// <returns>The minimum number of elements across all selected collections</returns>
        public virtual int GetElementCount()
        {
            var minLength = int.MaxValue;
            foreach (var value in _valueEntry.Values)
            {
                if (value == null)
                {
                    return 0;
                }
                minLength = System.Math.Min(minLength, value.Count);
            }
            return minLength == int.MaxValue ? 0 : minLength;
        }

        /// <summary>
        /// Gets an element at the specified index from the read-only list
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="elementIndex">The index of the element within the collection</param>
        /// <returns>The element at the specified index</returns>
        public virtual TElement GetElement(int targetIndex, int elementIndex)
        {
            if (targetIndex < 0 || targetIndex >= _valueEntry.Values.Count)
                throw new ArgumentOutOfRangeException(nameof(targetIndex));

            var collection = _valueEntry.Values[targetIndex];
            if (collection == null || elementIndex < 0 || elementIndex >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            return collection[elementIndex];
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
        /// Sets an element at the specified index in a collection. Always throws NotSupportedException.
        /// </summary>
        /// <param name="collection">The collection to set the element in</param>
        /// <param name="elementIndex">The index of the element to set</param>
        /// <param name="value">The value to set</param>
        /// <exception cref="NotSupportedException">Always thrown as read-only lists cannot be modified</exception>
        protected override void SetElementAt(TCollection collection, int elementIndex, TElement value)
        {
            throw new NotSupportedException("Readonly list cannot be modified");
        }
    }
}
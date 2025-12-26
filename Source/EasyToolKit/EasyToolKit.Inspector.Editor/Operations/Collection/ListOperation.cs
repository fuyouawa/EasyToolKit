using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Operation for IList{T} collections in the inspector system.
    /// Handles list-specific operations like indexed access, insertion, and removal.
    /// Implements IOrderedCollectionOperationResolver for position-based operations.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement IList{TElement})</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class ListOperation<TCollection, TElement> : OrderedCollectionOperationBase<TCollection, TElement>
        where TCollection : IList<TElement>
    {
        public ListOperation(Type ownerType) : base(ownerType)
        {
        }

        public override Type GetItemRuntimeType(ref TCollection collection)
        {
            return collection.GetType().GetArgumentsOfInheritedOpenGenericType(typeof(IList<>))[0];
        }

        public override int GetItemCount(ref TCollection collection)
        {
            return collection.Count;
        }

        /// <summary>
        /// Adds an element to the end of the list
        /// </summary>
        /// <param name="collection">The list collection</param>
        /// <param name="value">The element to add</param>
        public override void AddItem(ref TCollection collection, TElement value)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.Add(value);
        }

        /// <summary>
        /// Removes the first occurrence of an element from the list
        /// </summary>
        /// <param name="collection">The list collection</param>
        /// <param name="value">The element to remove</param>
        public override void RemoveItem(ref TCollection collection, TElement value)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.Remove(value);
        }

        public override TElement GetItemAt(ref TCollection collection, int index)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return collection[index];
        }

        /// <summary>
        /// Inserts an element into the list at the specified index
        /// </summary>
        /// <param name="collection">The list collection</param>
        /// <param name="index">The zero-based index at which value should be inserted</param>
        /// <param name="value">The element to insert</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0 or greater than the count of elements in the list</exception>
        public override void InsertItemAt(ref TCollection collection, int index, TElement value)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index < 0 || index > collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than or equal to the size of the collection.");

            collection.Insert(index, value);
        }

        /// <summary>
        /// Removes the element at the specified index from the list
        /// </summary>
        /// <param name="collection">The list collection</param>
        /// <param name="index">The zero-based index of the element to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0 or greater than or equal to the count of elements in the list</exception>
        public override void RemoveItemAt(ref TCollection collection, int index)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            collection.RemoveAt(index);
        }
    }
}

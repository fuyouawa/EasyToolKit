using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents an ordered collection entry that manages ordered collection values for multiple target objects.
    /// Extends CollectionEntry to provide ordered collection-specific operations such as inserting and removing items at specific indices.
    /// </summary>
    /// <typeparam name="TCollection">The type of the ordered collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public class OrderedCollectionEntry<TCollection, TItem> : CollectionEntry<TCollection, TItem>, IOrderedCollectionEntry<TCollection, TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedCollectionEntry{TCollection, TItem}"/> class.
        /// </summary>
        /// <param name="ownerElement">The value element that owns this ordered collection entry.</param>
        /// <exception cref="ArgumentNullException">Thrown when ownerElement is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when ordered collection operation resolver cannot be created.</exception>
        public OrderedCollectionEntry([NotNull] IValueElement ownerElement) : base(ownerElement)
        {
        }

        /// <summary>
        /// Gets the ordered collection operation that handles ordered collection-specific operations.
        /// </summary>
        protected new IOrderedCollectionOperation<TCollection, TItem> Operation =>
            (IOrderedCollectionOperation<TCollection, TItem>)base.Operation;

        public object GetWeakItemAt(int targetIndex, int itemIndex)
        {
            return GetItemAt(targetIndex, itemIndex);
        }

        /// <summary>
        /// Inserts a weakly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertWeakItemAt(int targetIndex, int itemIndex, object value)
        {
            InsertItemAt(targetIndex, itemIndex, (TItem)value);
        }

        /// <summary>
        /// Removes the item at the specified index from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index of the item to remove.</param>
        public void RemoveItemAt(int targetIndex, int itemIndex)
        {
            if (IsReadOnly)
            {
                Debug.LogError($"Ordered collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var collection = GetValue(targetIndex);
            var removedItem = Operation.GetItemAt(ref collection, itemIndex);

            using (var eventArgs = CollectionChangedEventArgs.Create(targetIndex, CollectionChangeType.RemoveAt, removedItem, itemIndex, CollectionChangedTiming.Before))
            {
                OnBeforeCollectionChanged(eventArgs);
            }

            collection = GetValue(targetIndex);
            Operation.RemoveItemAt(ref collection, itemIndex);
            SetValue(targetIndex, collection);
            MarkDirty();

            using (var eventArgs = CollectionChangedEventArgs.Create(targetIndex, CollectionChangeType.RemoveAt, removedItem, itemIndex, CollectionChangedTiming.After))
            {
                OnAfterCollectionChanged(eventArgs);
            }
        }

        public TItem GetItemAt(int targetIndex, int itemIndex)
        {
            var collection = GetValue(targetIndex);
            return Operation.GetItemAt(ref collection, itemIndex);
        }

        /// <summary>
        /// Inserts a strongly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertItemAt(int targetIndex, int itemIndex, TItem value)
        {
            if (IsReadOnly)
            {
                Debug.LogError($"Ordered collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            using (var eventArgs = CollectionChangedEventArgs.Create(targetIndex, CollectionChangeType.Insert, value, itemIndex, CollectionChangedTiming.Before))
            {
                OnBeforeCollectionChanged(eventArgs);
            }

            var collection = GetValue(targetIndex);
            Operation.InsertItemAt(ref collection, itemIndex, value);
            SetValue(targetIndex, collection);
            MarkDirty();

            using (var eventArgs = CollectionChangedEventArgs.Create(targetIndex, CollectionChangeType.Insert, value, itemIndex, CollectionChangedTiming.After))
            {
                OnAfterCollectionChanged(eventArgs);
            }
        }
    }
}

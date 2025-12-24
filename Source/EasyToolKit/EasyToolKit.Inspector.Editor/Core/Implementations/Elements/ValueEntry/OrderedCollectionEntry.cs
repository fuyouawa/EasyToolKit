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
        private readonly IOrderedCollectionOperation<TCollection, TItem> _operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedCollectionEntry{TCollection, TItem}"/> class.
        /// </summary>
        /// <param name="ownerElement">The value element that owns this ordered collection entry.</param>
        /// <exception cref="ArgumentNullException">Thrown when ownerElement is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when ordered collection operation resolver cannot be created.</exception>
        public OrderedCollectionEntry([NotNull] IValueElement ownerElement) : base(ownerElement)
        {
            _operation = (IOrderedCollectionOperation<TCollection, TItem>)base.Operation;
        }

        /// <summary>
        /// Gets the ordered collection operation that handles ordered collection-specific operations.
        /// </summary>
        protected IOrderedCollectionOperation<TCollection, TItem> Operation => _operation;

        /// <summary>
        /// Inserts a weakly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertWeakItem(int targetIndex, int itemIndex, object value)
        {
            InsertItem(targetIndex, itemIndex, (TItem)value);
        }

        /// <summary>
        /// Removes the item at the specified index from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index of the item to remove.</param>
        public void RemoveItem(int targetIndex, int itemIndex)
        {
            if (IsReadOnly)
            {
                Debug.LogError($"Ordered collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var collection = GetValue(targetIndex);
            _operation.RemoveItemAt(ref collection, itemIndex);
            SetValue(targetIndex, collection);
            MarkDirty();
        }

        /// <summary>
        /// Inserts a strongly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertItem(int targetIndex, int itemIndex, TItem value)
        {
            if (IsReadOnly)
            {
                Debug.LogError($"Ordered collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var collection = GetValue(targetIndex);
            _operation.InsertItemAt(ref collection, itemIndex, value);
            SetValue(targetIndex, collection);
            MarkDirty();
        }
    }
}

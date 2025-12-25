using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents a collection entry that manages collection values for multiple target objects.
    /// Extends ValueEntry to provide collection-specific operations such as adding and removing items.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public class CollectionEntry<TCollection, TItem> : ValueEntry<TCollection>, ICollectionEntry<TCollection, TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEntry{TCollection, TItem}"/> class.
        /// </summary>
        /// <param name="ownerElement">The value element that owns this collection entry.</param>
        /// <exception cref="ArgumentNullException">Thrown when ownerElement is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when collection operation resolver cannot be created.</exception>
        public CollectionEntry([NotNull] IValueElement ownerElement) : base(ownerElement)
        {
        }

        /// <summary>
        /// Gets the type of items in the collection.
        /// </summary>
        public Type ItemType => typeof(TItem);

        public Type RuntimeItemType
        {
            get
            {
                if (RuntimeValueType != null)
                {
                    var collection = WeakSmartValue;
                    return Operation.GetItemRuntimeType(ref collection);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the collection operation that handles collection-specific operations.
        /// </summary>
        protected new ICollectionOperation<TCollection, TItem> Operation => (ICollectionOperation<TCollection, TItem>)base.Operation;

        /// <summary>
        /// Adds a weakly-typed item to the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to add.</param>
        /// <returns>The resulting collection after the operation.</returns>
        public void AddWeakItem(int targetIndex, object value)
        {
            AddItem(targetIndex, (TItem)value);
        }

        /// <summary>
        /// Adds a strongly-typed item to the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to add.</param>
        /// <returns>The resulting collection after the operation.</returns>
        public void AddItem(int targetIndex, TItem value)
        {
            if (IsReadOnly)
            {
                Debug.LogError($"Collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var collection = GetValue(targetIndex);
            Operation.AddItem(ref collection, value);
            SetValue(targetIndex, collection);
            MarkDirty();
        }

        /// <summary>
        /// Removes a weakly-typed item from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to remove.</param>
        public void RemoveWeakItem(int targetIndex, object value)
        {
            RemoveItem(targetIndex, (TItem)value);
        }

        /// <summary>
        /// Removes a strongly-typed item from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to remove.</param>
        public void RemoveItem(int targetIndex, TItem value)
        {
            if (IsReadOnly)
            {
                Debug.LogWarning($"Collection '{OwnerElement.Path}' cannot be edited.");
                return;
            }

            var collection = GetValue(targetIndex);
            Operation.RemoveItem(ref collection, value);
            SetValue(targetIndex, collection);
            MarkDirty();
        }
    }
}

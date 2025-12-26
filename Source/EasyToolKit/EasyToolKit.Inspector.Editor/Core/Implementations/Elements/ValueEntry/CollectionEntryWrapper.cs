using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe wrapper for a collection entry that exposes a more derived collection and item type.
    /// Delegates all operations to the underlying <see cref="ICollectionEntry{TBaseCollection, TBaseItem}"/> while
    /// providing compile-time type safety for the more specific collection and item types.
    /// </summary>
    /// <typeparam name="TCollection">The derived collection type exposed by this wrapper.</typeparam>
    /// <typeparam name="TItem">The derived item type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseCollection">The base collection type stored in the underlying collection entry.</typeparam>
    /// <typeparam name="TBaseItem">The base item type stored in the underlying collection entry.</typeparam>
    public class CollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem> :
        ValueEntryWrapper<TCollection, TBaseCollection>,
        ICollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>
        where TBaseCollection : notnull
        where TCollection : TBaseCollection
        where TItem : TBaseItem
    {
        private readonly ICollectionEntry<TBaseCollection, TBaseItem> _baseCollectionEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEntryWrapper{TCollection, TItem, TBaseCollection, TBaseItem}"/> class.
        /// </summary>
        /// <param name="baseCollectionEntry">The underlying collection entry to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when baseCollectionEntry is null.</exception>
        public CollectionEntryWrapper([NotNull] ICollectionEntry<TBaseCollection, TBaseItem> baseCollectionEntry)
            : base(baseCollectionEntry)
        {
            _baseCollectionEntry = baseCollectionEntry ?? throw new ArgumentNullException(nameof(baseCollectionEntry));
        }

        /// <summary>
        /// Gets the type of items in the collection.
        /// </summary>
        public Type ItemType => _baseCollectionEntry.ItemType;

        public Type RuntimeItemType => _baseCollectionEntry.RuntimeItemType;

        public event EventHandler<CollectionChangedEventArgs> BeforeCollectionChanged
        {
            add => _baseCollectionEntry.BeforeCollectionChanged += value;
            remove => _baseCollectionEntry.BeforeCollectionChanged -= value;
        }
        public event EventHandler<CollectionChangedEventArgs> AfterCollectionChanged
        {
            add => _baseCollectionEntry.AfterCollectionChanged += value;
            remove => _baseCollectionEntry.AfterCollectionChanged -= value;
        }

        /// <summary>
        /// Gets the underlying collection entry.
        /// </summary>
        ICollectionEntry<TBaseCollection, TBaseItem> ICollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>.BaseValueEntry =>
            _baseCollectionEntry;

        public int GetItemCount(int targetIndex)
        {
            return _baseCollectionEntry.GetItemCount(targetIndex);
        }

        /// <summary>
        /// Adds a weakly-typed item to the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to add.</param>
        public void AddWeakItem(int targetIndex, object value)
        {
            _baseCollectionEntry.AddWeakItem(targetIndex, value);
        }

        /// <summary>
        /// Adds a strongly-typed item to the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to add.</param>
        public void AddItem(int targetIndex, TItem value)
        {
            _baseCollectionEntry.AddItem(targetIndex, value);
        }

        /// <summary>
        /// Removes a weakly-typed item from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to remove.</param>
        public void RemoveWeakItem(int targetIndex, object value)
        {
            _baseCollectionEntry.RemoveWeakItem(targetIndex, value);
        }

        /// <summary>
        /// Removes a strongly-typed item from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="value">The item to remove.</param>
        public void RemoveItem(int targetIndex, TItem value)
        {
            _baseCollectionEntry.RemoveItem(targetIndex, value);
        }
    }
}

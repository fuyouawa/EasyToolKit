using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe wrapper for an ordered collection entry that exposes a more derived collection and item type.
    /// Delegates all operations to the underlying <see cref="IOrderedCollectionEntry{TBaseCollection, TBaseItem}"/> while
    /// providing compile-time type safety for the more specific collection and item types.
    /// </summary>
    /// <typeparam name="TCollection">The derived ordered collection type exposed by this wrapper.</typeparam>
    /// <typeparam name="TItem">The derived item type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseCollection">The base ordered collection type stored in the underlying collection entry.</typeparam>
    /// <typeparam name="TBaseItem">The base item type stored in the underlying collection entry.</typeparam>
    public class OrderedCollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem> :
        CollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>,
        IOrderedCollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>
        where TBaseCollection : notnull
        where TCollection : TBaseCollection
        where TItem : TBaseItem
    {
        private readonly IOrderedCollectionEntry<TBaseCollection, TBaseItem> _baseOrderedCollectionEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedCollectionEntryWrapper{TCollection, TItem, TBaseCollection, TBaseItem}"/> class.
        /// </summary>
        /// <param name="baseOrderedCollectionEntry">The underlying ordered collection entry to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when baseOrderedCollectionEntry is null.</exception>
        public OrderedCollectionEntryWrapper([NotNull] IOrderedCollectionEntry<TBaseCollection, TBaseItem> baseOrderedCollectionEntry)
            : base(baseOrderedCollectionEntry)
        {
            _baseOrderedCollectionEntry = baseOrderedCollectionEntry ?? throw new ArgumentNullException(nameof(baseOrderedCollectionEntry));
        }

        /// <summary>
        /// Gets the underlying ordered collection entry.
        /// </summary>
        IOrderedCollectionEntry<TBaseCollection, TBaseItem> IOrderedCollectionEntryWrapper<TCollection, TItem, TBaseCollection, TBaseItem>.BaseValueEntry =>
            _baseOrderedCollectionEntry;

        public object GetWeakItemAt(int targetIndex, int itemIndex)
        {
            return _baseOrderedCollectionEntry.GetWeakItemAt(targetIndex, itemIndex);
        }

        /// <summary>
        /// Inserts a weakly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertWeakItemAt(int targetIndex, int itemIndex, object value)
        {
            _baseOrderedCollectionEntry.InsertWeakItemAt(targetIndex, itemIndex, value);
        }

        /// <summary>
        /// Removes the item at the specified index from the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index of the item to remove.</param>
        public void RemoveItemAt(int targetIndex, int itemIndex)
        {
            _baseOrderedCollectionEntry.RemoveItemAt(targetIndex, itemIndex);
        }

        public TItem GetItemAt(int targetIndex, int itemIndex)
        {
            return (TItem)_baseOrderedCollectionEntry.GetItemAt(targetIndex, itemIndex);
        }

        /// <summary>
        /// Inserts a strongly-typed item at the specified index in the collection for the specified target.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="itemIndex">The zero-based index at which to insert the item.</param>
        /// <param name="value">The item to insert.</param>
        public void InsertItemAt(int targetIndex, int itemIndex, TItem value)
        {
            _baseOrderedCollectionEntry.InsertItemAt(targetIndex, itemIndex, value);
        }
    }
}

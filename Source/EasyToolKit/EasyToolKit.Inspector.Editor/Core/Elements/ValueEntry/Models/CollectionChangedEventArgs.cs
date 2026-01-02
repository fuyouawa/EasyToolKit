using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the type of collection change.
    /// </summary>
    public enum CollectionChangeType
    {
        /// <summary>
        /// An item was added to the collection.
        /// </summary>
        Add,

        /// <summary>
        /// An item was removed from the collection.
        /// </summary>
        Remove,

        /// <summary>
        /// An item was inserted at a specific index in the collection.
        /// </summary>
        Insert,

        /// <summary>
        /// An item was removed from a specific index in the collection.
        /// </summary>
        RemoveAt,

        /// <summary>
        /// The collection was cleared.
        /// </summary>
        Clear,
    }

    /// <summary>
    /// Specifies the timing of the collection changed event.
    /// </summary>
    public enum CollectionChangedTiming
    {
        /// <summary>
        /// The event is raised before the collection is changed.
        /// </summary>
        Before,

        /// <summary>
        /// The event is raised after the collection has been changed.
        /// </summary>
        After,
    }

    /// <summary>
    /// Provides data for collection change events.
    /// </summary>
    [MustDisposeResource]
    public class CollectionChangedEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Gets the zero-based index of the target object.
        /// </summary>
        public int TargetIndex { get; private set; }

        /// <summary>
        /// Gets the type of collection change.
        /// </summary>
        public CollectionChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the item involved in the change operation.
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Gets the zero-based index of the item in the collection, if applicable.
        /// </summary>
        public int? ItemIndex { get; private set; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public CollectionChangedTiming Timing { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionChangedEventArgs"/> class from the object pool.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="changeType">The type of collection change.</param>
        /// <param name="item">The item involved in the change operation.</param>
        /// <param name="itemIndex">The zero-based index of the item in the collection (if applicable).</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        /// <returns>A new or reused instance of <see cref="CollectionChangedEventArgs"/>.</returns>
        public static CollectionChangedEventArgs Create(int targetIndex, CollectionChangeType changeType, object item, int? itemIndex, CollectionChangedTiming timing)
        {
            var args = EditorPoolUtility.Rent<CollectionChangedEventArgs>();
            args.TargetIndex = targetIndex;
            args.ChangeType = changeType;
            args.Item = item;
            args.ItemIndex = itemIndex;
            args.Timing = timing;
            return args;
        }

        /// <summary>
        /// Releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            EditorPoolUtility.Release(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
            TargetIndex = 0;
            ChangeType = default;
            Item = null;
            ItemIndex = null;
            Timing = default;
        }
    }
}

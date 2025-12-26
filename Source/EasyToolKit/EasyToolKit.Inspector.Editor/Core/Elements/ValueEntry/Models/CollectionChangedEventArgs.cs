using System;

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
    public class CollectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="changeType">The type of collection change.</param>
        /// <param name="item">The item involved in the change operation.</param>
        /// <param name="itemIndex">The zero-based index of the item in the collection (if applicable).</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        public CollectionChangedEventArgs(int targetIndex, CollectionChangeType changeType, object item, int? itemIndex, CollectionChangedTiming timing)
        {
            TargetIndex = targetIndex;
            ChangeType = changeType;
            Item = item;
            ItemIndex = itemIndex;
            Timing = timing;
        }

        /// <summary>
        /// Gets the zero-based index of the target object.
        /// </summary>
        public int TargetIndex { get; }

        /// <summary>
        /// Gets the type of collection change.
        /// </summary>
        public CollectionChangeType ChangeType { get; }

        /// <summary>
        /// Gets the item involved in the change operation.
        /// </summary>
        public object Item { get; }

        /// <summary>
        /// Gets the zero-based index of the item in the collection, if applicable.
        /// </summary>
        public int? ItemIndex { get; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public CollectionChangedTiming Timing { get; }
    }
}

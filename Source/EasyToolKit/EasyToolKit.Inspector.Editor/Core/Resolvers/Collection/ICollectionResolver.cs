using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving collection properties in the inspector.
    /// Provides basic collection operations and metadata for collection types.
    /// </summary>
    public interface ICollectionResolver
    {
        /// <summary>
        /// Gets whether the collection is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the type of the collection.
        /// </summary>
        Type CollectionType { get; }

        /// <summary>
        /// Gets the type of elements in the collection.
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Queues an element to be added to the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        void QueueAddElement(int targetIndex, object value);

        /// <summary>
        /// Queues an element to be removed from the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        void QueueRemoveElement(int targetIndex, object value);
    }
}

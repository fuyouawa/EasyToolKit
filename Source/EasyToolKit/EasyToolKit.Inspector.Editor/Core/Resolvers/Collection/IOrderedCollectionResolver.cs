namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolving ordered collection properties in the inspector.
    /// Extends <see cref="ICollectionResolver"/> with position-based operations for ordered collections.
    /// </summary>
    public interface IOrderedCollectionResolver : ICollectionResolver
    {
        /// <summary>
        /// Queues an element to be inserted into the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection where the element should be inserted.</param>
        /// <param name="value">The value to insert into the collection.</param>
        void QueueInsertElementAt(int targetIndex, int index, object value);

        /// <summary>
        /// Queues an element to be removed from the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection from which to remove the element.</param>
        void QueueRemoveElementAt(int targetIndex, int index);
    }
}

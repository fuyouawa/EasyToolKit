using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base class for resolving ordered collection properties in the inspector.
    /// Extends <see cref="CollectionResolverBase{TCollection,TElement}"/> with position-based operations for ordered collections.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement <see cref="IEnumerable{TElement}"/>).</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public abstract class OrderedCollectionResolverBase<TCollection, TElement> :
        CollectionResolverBase<TCollection, TElement>,
        IOrderedCollectionResolver
        where TCollection : IEnumerable<TElement>
    {
        /// <summary>
        /// Queues an element to be inserted into the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection where the element should be inserted.</param>
        /// <param name="value">The value to insert into the collection.</param>
        public void QueueInsertElementAt(int targetIndex, int index, object value)
        {
            EnqueueChange(() => InsertElementAt(targetIndex, index, (TElement)value));
        }

        /// <summary>
        /// Queues an element to be removed from the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection from which to remove the element.</param>
        public void QueueRemoveElementAt(int targetIndex, int index)
        {
            EnqueueChange(() => RemoveElementAt(targetIndex, index));
        }

        /// <summary>
        /// Abstract method to insert an element into the collection at a specific index.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection where the element should be inserted.</param>
        /// <param name="value">The value to insert into the collection.</param>
        protected abstract void InsertElementAt(int targetIndex, int index, TElement value);

        /// <summary>
        /// Abstract method to remove an element from the collection at a specific index.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection from which to remove the element.</param>
        protected abstract void RemoveElementAt(int targetIndex, int index);
    }
}

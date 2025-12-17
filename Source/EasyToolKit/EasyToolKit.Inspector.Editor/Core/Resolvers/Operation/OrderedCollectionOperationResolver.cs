using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for ordered collection operation resolvers.
    /// Provides position-based collection operations with index-based insertion and removal.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public abstract class OrderedCollectionOperationResolver<TCollection, TElement> : CollectionOperationResolver<TCollection, TElement>, IOrderedCollectionOperationResolver
    {
        void IOrderedCollectionOperationResolver.InsertElementAt(int targetIndex, int index, object value)
        {
            InsertElementAt(targetIndex, index, (TElement)value);
        }

        void IOrderedCollectionOperationResolver.RemoveElementAt(int targetIndex, int index)
        {
            RemoveElementAt(targetIndex, index);
        }

        /// <summary>
        /// Actually inserts an element into the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        protected abstract void InsertElementAt(int targetIndex, int index, TElement value);

        /// <summary>
        /// Actually removes an element from the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        protected abstract void RemoveElementAt(int targetIndex, int index);
    }
}

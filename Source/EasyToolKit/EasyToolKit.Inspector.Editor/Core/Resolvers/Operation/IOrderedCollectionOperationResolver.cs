using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for handling ordered collection operations in the inspector system.
    /// Extends <see cref="ICollectionOperationResolver"/> with position-based operations.
    /// </summary>
    public interface IOrderedCollectionOperationResolver : ICollectionOperationResolver
    {
        /// <summary>
        /// Actually inserts an element into the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        void InsertElementAt(int targetIndex, int index, object value);

        /// <summary>
        /// Actually removes an element from the collection at the specified index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        void RemoveElementAt(int targetIndex, int index);
    }
}

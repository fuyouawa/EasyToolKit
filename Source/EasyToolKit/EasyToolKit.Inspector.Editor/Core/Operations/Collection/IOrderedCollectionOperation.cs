using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for handling ordered collection operations in the inspector system.
    /// Extends <see cref="ICollectionOperation"/> with position-based operations.
    /// </summary>
    public interface IOrderedCollectionOperation : ICollectionOperation
    {
        /// <summary>
        /// Inserts an element into the collection at the specified index
        /// </summary>
        /// <param name="collection">collection target</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        void InsertWeakElementAt(ref object collection, int index, object value);

        /// <summary>
        /// Removes an element from the collection at the specified index
        /// </summary>
        /// <param name="collection">collection target</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        void RemoveWeakElementAt(ref object collection, int index);
    }

    /// <summary>
    /// Generic interface for ordered collection operations with type safety
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <typeparam name="TElement">Element type</typeparam>
    public interface IOrderedCollectionOperation<TOwner, TCollection, TElement> : IOrderedCollectionOperation, ICollectionOperation<TOwner, TCollection, TElement>
    {
        /// <summary>
        /// Inserts an element into the collection at the specified index with type safety
        /// </summary>
        /// <param name="collection">Collection target</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        void InsertElementAt(ref TCollection collection, int index, TElement value);

        /// <summary>
        /// Removes an element from the collection at the specified index with type safety
        /// </summary>
        /// <param name="collection">Collection target</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        void RemoveElementAt(ref TCollection collection, int index);
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for handling collection operations in the inspector system.
    /// Focuses purely on collection manipulation without property structure concerns.
    /// </summary>
    public interface ICollectionOperationResolver
    {
        /// <summary>
        /// Gets whether the collection is read-only
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        Type CollectionType { get; }

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Actually adds an element to the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to add to the collection</param>
        void AddElement(int targetIndex, object value);

        /// <summary>
        /// Actually removes an element from the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to remove from the collection</param>
        void RemoveElement(int targetIndex, object value);

        /// <summary>
        /// Gets an element at the specified index from a collection
        /// </summary>
        /// <param name="collection">The collection to get the element from</param>
        /// <param name="elementIndex">The index of the element</param>
        /// <returns>The element at the specified index</returns>
        object GetElementAt(object collection, int elementIndex);

        /// <summary>
        /// Sets an element at the specified index in a collection
        /// </summary>
        /// <param name="collection">The collection to set the element in</param>
        /// <param name="elementIndex">The index of the element to set</param>
        /// <param name="value">The value to set</param>
        void SetElementAt(object collection, int elementIndex, object value);
    }
}

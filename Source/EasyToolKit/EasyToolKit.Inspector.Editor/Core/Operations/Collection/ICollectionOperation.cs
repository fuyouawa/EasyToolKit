using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Collection operation interface, inherits from IPropertyOperation
    /// </summary>
    public interface ICollectionOperation : IValueOperation
    {
        /// <summary>
        /// Collection type
        /// </summary>
        Type CollectionType { get; }

        /// <summary>
        /// Element type
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Adds an element to the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to add</param>
        void AddWeakElement(ref object collection, object value);

        /// <summary>
        /// Removes an element from the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to remove</param>
        void RemoveWeakElement(ref object collection, object value);
    }

    public interface ICollectionOperation<TCollection, TElement> : ICollectionOperation, IValueOperation<TCollection>
    {
        void AddElement(ref TCollection collection, TElement value);
        void RemoveElement(ref TCollection collection, TElement value);
    }
}

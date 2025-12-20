using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Generic abstract base class for ordered collection operations with type safety
    /// </summary>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <typeparam name="TElement">Element type</typeparam>
    public abstract class OrderedCollectionOperationBase<TCollection, TElement> : CollectionOperationBase<TCollection, TElement>, IOrderedCollectionOperation<TCollection, TElement>
    {
        protected OrderedCollectionOperationBase(Type ownerType) : base(ownerType)
        {
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index with type safety
        /// </summary>
        /// <param name="collection">Collection target</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        public abstract void InsertElementAt(ref TCollection collection, int index, TElement value);

        /// <summary>
        /// Removes an element from the collection at the specified index with type safety
        /// </summary>
        /// <param name="collection">Collection target</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        public abstract void RemoveElementAt(ref TCollection collection, int index);

        /// <summary>
        /// Inserts an element into the collection at the specified index
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="index">The position in the collection where the element should be inserted</param>
        /// <param name="value">The value to insert into the collection</param>
        public virtual void InsertWeakElementAt(ref object collection, int index, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TElement)value;
            InsertElementAt(ref castCollection, index, castValue);
            collection = castCollection;
        }

        /// <summary>
        /// Removes an element from the collection at the specified index
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="index">The position in the collection from which to remove the element</param>
        public virtual void RemoveWeakElementAt(ref object collection, int index)
        {
            var castCollection = (TCollection)collection;
            RemoveElementAt(ref castCollection, index);
            collection = castCollection;
        }
    }
}

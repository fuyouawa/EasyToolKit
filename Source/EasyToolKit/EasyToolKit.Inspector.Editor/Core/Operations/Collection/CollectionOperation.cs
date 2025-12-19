using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for collection operations
    /// </summary>
    public abstract class CollectionOperation : ICollectionOperation
    {
        protected CollectionOperation(Type ownerType)
        {
            OwnerType = ownerType;
        }

        /// <summary>
        /// Whether the collection is read-only
        /// </summary>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Owner type
        /// </summary>
        public virtual Type OwnerType { get; }

        /// <summary>
        /// Value type (collection type)
        /// </summary>
        public abstract Type ValueType { get; }

        /// <summary>
        /// Collection type
        /// </summary>
        public abstract Type CollectionType { get; }

        /// <summary>
        /// Element type
        /// </summary>
        public abstract Type ElementType { get; }

        /// <summary>
        /// Gets the collection value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Collection value</returns>
        public virtual object GetWeakValue(ref object owner)
        {
            throw new NotSupportedException("Getting values is not supported for collection operations.");
        }

        /// <summary>
        /// Sets the collection value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Collection to set</param>
        public virtual void SetWeakValue(ref object owner, object value)
        {
            throw new NotSupportedException("Setting values is not supported for collection operations.");
        }

        /// <summary>
        /// Adds an element to the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to add</param>
        public abstract void AddWeakElement(ref object collection, object value);

        /// <summary>
        /// Removes an element from the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to remove</param>
        public abstract void RemoveWeakElement(ref object collection, object value);
    }

    /// <summary>
    /// Generic abstract base class for collection operations with type safety
    /// </summary>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <typeparam name="TElement">Element type</typeparam>
    public abstract class CollectionOperation<TCollection, TElement> : CollectionOperation, ICollectionOperation<TCollection, TElement>
    {
        protected CollectionOperation(Type ownerType) : base(ownerType)
        {
        }

        /// <summary>
        /// Value type (collection type)
        /// </summary>
        public override Type ValueType => typeof(TCollection);

        /// <summary>
        /// Collection type
        /// </summary>
        public override Type CollectionType => typeof(TCollection);

        /// <summary>
        /// Element type
        /// </summary>
        public override Type ElementType => typeof(TElement);

        /// <summary>
        /// Gets the collection value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Collection value</returns>
        public virtual TCollection GetValue(ref object owner)
        {
            throw new NotSupportedException("Getting values is not supported for collection operations.");
        }

        /// <summary>
        /// Sets the collection value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Collection to set</param>
        public virtual void SetValue(ref object owner, TCollection value)
        {
            throw new NotSupportedException("Setting values is not supported for collection operations.");
        }

        /// <summary>
        /// Adds an element to the collection with type safety
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to add</param>
        public abstract void AddElement(ref TCollection collection, TElement value);

        /// <summary>
        /// Removes an element from the collection with type safety
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to remove</param>
        public abstract void RemoveElement(ref TCollection collection, TElement value);

        /// <summary>
        /// Gets the collection value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Collection value</returns>
        public override object GetWeakValue(ref object owner)
        {
            return GetValue(ref owner);
        }

        /// <summary>
        /// Sets the collection value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Collection to set</param>
        public override void SetWeakValue(ref object owner, object value)
        {
            var castValue = (TCollection)value;
            SetValue(ref owner, castValue);
        }

        /// <summary>
        /// Adds an element to the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to add</param>
        public override void AddWeakElement(ref object collection, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TElement)value;
            AddElement(ref castCollection, castValue);
            collection = castCollection;
        }

        /// <summary>
        /// Removes an element from the collection
        /// </summary>
        /// <param name="collection">Collection object</param>
        /// <param name="value">Element to remove</param>
        public override void RemoveWeakElement(ref object collection, object value)
        {
            var castCollection = (TCollection)collection;
            var castValue = (TElement)value;
            RemoveElement(ref castCollection, castValue);
            collection = castCollection;
        }
    }
}

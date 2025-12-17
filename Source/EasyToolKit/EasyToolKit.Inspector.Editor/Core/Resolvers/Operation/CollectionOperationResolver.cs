using System;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for collection operation resolvers in the inspector system.
    /// Provides common functionality for collection manipulation without property structure concerns.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public abstract class CollectionOperationResolver<TCollection, TElement> : ICollectionOperationResolver
    {
        private Action _changeAction;
        private InspectorProperty _property;

        /// <summary>
        /// Gets whether the collection is read-only
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        public abstract Type CollectionType { get; }

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public Type ElementType => typeof(TElement);

        /// <summary>
        /// Gets or sets the associated inspector property
        /// </summary>
        public InspectorProperty Property
        {
            get => _property;
            set => _property = value;
        }


        void ICollectionOperationResolver.AddElement(int targetIndex, object value)
        {
            AddElement(targetIndex, (TElement)value);
        }

        void ICollectionOperationResolver.RemoveElement(int targetIndex, object value)
        {
            RemoveElement(targetIndex, (TElement)value);
        }

        object ICollectionOperationResolver.GetElementAt(object collection, int elementIndex)
        {
            return GetElementAt((TCollection)collection, elementIndex);
        }

        void ICollectionOperationResolver.SetElementAt(object collection, int elementIndex, object value)
        {
            SetElementAt((TCollection)collection, elementIndex, (TElement)value);
        }

        /// <summary>
        /// Actually adds an element to the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to add to the collection</param>
        protected abstract void AddElement(int targetIndex, TElement value);

        /// <summary>
        /// Actually removes an element from the collection at the specified target index
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context</param>
        /// <param name="value">The value to remove from the collection</param>
        protected abstract void RemoveElement(int targetIndex, TElement value);

        /// <summary>
        /// Gets an element at the specified index from a collection
        /// </summary>
        /// <param name="collection">The collection to get the element from</param>
        /// <param name="elementIndex">The index of the element</param>
        /// <returns>The element at the specified index</returns>
        protected abstract TElement GetElementAt(TCollection collection, int elementIndex);

        /// <summary>
        /// Sets an element at the specified index in a collection
        /// </summary>
        /// <param name="collection">The collection to set the element in</param>
        /// <param name="elementIndex">The index of the element to set</param>
        /// <param name="value">The value to set</param>
        protected abstract void SetElementAt(TCollection collection, int elementIndex, TElement value);
    }
}

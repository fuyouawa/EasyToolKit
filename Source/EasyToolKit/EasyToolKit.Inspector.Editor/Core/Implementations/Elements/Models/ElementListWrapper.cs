using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe wrapper for an element list that exposes a more derived element type.
    /// Delegates all operations to the underlying <see cref="IElementList{TBaseElement}"/> while
    /// providing compile-time type safety for the more specific element type.
    /// </summary>
    /// <typeparam name="TElement">The derived element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseElement">The base element type stored in the underlying list.</typeparam>
    public class ElementListWrapper<TElement, TBaseElement> : IElementListWrapper<TElement, TBaseElement>
        where TBaseElement : IElement
        where TElement : TBaseElement
    {
        private readonly IElementList<TBaseElement> _baseList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementListWrapper{TBaseElement, TElement}"/> class.
        /// </summary>
        /// <param name="baseList">The underlying element list to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when baseList is null.</exception>
        public ElementListWrapper([NotNull] IElementList<TBaseElement> baseList)
        {
            _baseList = baseList ?? throw new ArgumentNullException(nameof(baseList));
        }

        /// <summary>
        /// Occurs before the element list is moved.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> BeforeElementMoved
        {
            add => _baseList.BeforeElementMoved += value;
            remove => _baseList.BeforeElementMoved -= value;
        }

        /// <summary>
        /// Occurs after the element list is moved.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> AfterElementMoved
        {
            add => _baseList.AfterElementMoved += value;
            remove => _baseList.AfterElementMoved -= value;
        }

        /// <summary>
        /// Gets the element that owns this list.
        /// </summary>
        public IElement OwnerElement => _baseList.OwnerElement;

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _baseList.Count;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public TElement this[int index] => (TElement)_baseList[index];

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        public TElement this[string name] => (TElement)_baseList[name];

        /// <summary>
        /// Gets the underlying element list.
        /// </summary>
        public IElementList<TBaseElement> BaseList => _baseList;

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="element">The element to insert.</param>
        public void Insert(int index, TElement element)
        {
            _baseList.Insert(index, element);
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _baseList.RemoveAt(index);
        }

        /// <summary>
        /// Gets the zero-based index of the first occurrence of an element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if not found.</returns>
        public int IndexOf(string name)
        {
            return _baseList.IndexOf(name);
        }

        /// <summary>
        /// Gets the zero-based index of the specified element.
        /// </summary>
        /// <param name="element">The element to locate.</param>
        /// <returns>The zero-based index of the element, or -1 if not found.</returns>
        public int IndexOf(TElement element)
        {
            return _baseList.IndexOf(element);
        }

        /// <summary>
        /// Gets the full path of the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The full path of the element.</returns>
        public string GetPath(int index)
        {
            return _baseList.GetPath(index);
        }

        /// <summary>
        /// Updates the collection.
        /// </summary>
        public void Update()
        {
            _baseList.Update();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (var element in _baseList)
            {
                yield return (TElement)element;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

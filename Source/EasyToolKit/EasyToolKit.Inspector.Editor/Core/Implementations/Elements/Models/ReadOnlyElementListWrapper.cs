using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe read-only wrapper for an element list that exposes a more derived element type.
    /// Delegates all read operations to the underlying <see cref="IReadOnlyElementList{TBaseElement}"/> while
    /// providing compile-time type safety for the more specific element type.
    /// </summary>
    /// <typeparam name="TElement">The derived element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseElement">The base element type stored in the underlying list.</typeparam>
    public class ReadOnlyElementListWrapper<TElement, TBaseElement> : IReadOnlyElementListWrapper<TElement, TBaseElement>
        where TBaseElement : IElement
        where TElement : TBaseElement
    {
        private readonly IReadOnlyElementList<TBaseElement> _baseList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyElementListWrapper{TBaseElement, TElement}"/> class.
        /// </summary>
        /// <param name="baseList">The underlying element list to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when baseList is null.</exception>
        public ReadOnlyElementListWrapper([NotNull] IReadOnlyElementList<TBaseElement> baseList)
        {
            _baseList = baseList ?? throw new ArgumentNullException(nameof(baseList));
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
        public IReadOnlyElementList<TBaseElement> BaseList => _baseList;

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

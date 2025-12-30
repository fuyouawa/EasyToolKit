using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe read-only wrapper for an element list that boxes derived elements to base type.
    /// Delegates all read operations to the underlying <see cref="IReadOnlyElementList{TDerivedElement}"/> while
    /// exposing them as the more general base type.
    /// </summary>
    /// <typeparam name="TBaseElement">The base element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TDerivedElement">The derived element type stored in the underlying list.</typeparam>
    public class ReadOnlyElementListBoxedWrapper<TBaseElement, TDerivedElement> : IReadOnlyElementListBoxedWrapper<TBaseElement, TDerivedElement>
        where TBaseElement : IElement
        where TDerivedElement : TBaseElement
    {
        private readonly IReadOnlyElementList<TDerivedElement> _derivedList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyElementListBoxedWrapper{TBaseElement, TDerivedElement}"/> class.
        /// </summary>
        /// <param name="derivedList">The underlying element list that stores derived elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when derivedList is null.</exception>
        public ReadOnlyElementListBoxedWrapper([NotNull] IReadOnlyElementList<TDerivedElement> derivedList)
        {
            _derivedList = derivedList ?? throw new ArgumentNullException(nameof(derivedList));
        }

        /// <summary>
        /// Gets the element that owns this list.
        /// </summary>
        public IElement OwnerElement => _derivedList.OwnerElement;

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _derivedList.Count;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public TBaseElement this[int index] => _derivedList[index];

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        public TBaseElement this[string name] => _derivedList[name];

        /// <summary>
        /// Gets the underlying element list that stores derived elements.
        /// </summary>
        public IReadOnlyElementList<TDerivedElement> DerivedList => _derivedList;

        /// <summary>
        /// Gets the zero-based index of the first occurrence of an element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if not found.</returns>
        public int IndexOf(string name)
        {
            return _derivedList.IndexOf(name);
        }

        /// <summary>
        /// Gets the zero-based index of the specified element.
        /// </summary>
        /// <param name="element">The element to locate.</param>
        /// <returns>The zero-based index of the element, or -1 if not found.</returns>
        public int IndexOf(TBaseElement element)
        {
            if (element is TDerivedElement derivedElement)
            {
                return _derivedList.IndexOf(derivedElement);
            }
            return -1;
        }

        /// <summary>
        /// Gets the full path of the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The full path of the element.</returns>
        public string GetPath(int index)
        {
            return _derivedList.GetPath(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<TBaseElement> GetEnumerator()
        {
            foreach (var element in _derivedList)
            {
                yield return element;
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

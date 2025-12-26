using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Type-safe wrapper for an element list that boxes derived elements to base type.
    /// Delegates all operations to the underlying <see cref="IElementList{TDerivedElement}"/> while
    /// providing compile-time type safety for the more general base element type.
    /// </summary>
    /// <typeparam name="TBaseElement">The base element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TDerivedElement">The derived element type stored in the underlying list.</typeparam>
    public class ElementListBoxedWrapper<TBaseElement, TDerivedElement> :
        ReadOnlyElementListBoxedWrapper<TBaseElement, TDerivedElement>,
        IElementListBoxedWrapper<TBaseElement, TDerivedElement>
        where TBaseElement : IElement
        where TDerivedElement : TBaseElement
    {
        private readonly IElementList<TDerivedElement> _derivedList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementListBoxedWrapper{TBaseElement, TDerivedElement}"/> class.
        /// </summary>
        /// <param name="derivedList">The underlying element list that stores derived elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when derivedList is null.</exception>
        public ElementListBoxedWrapper([NotNull] IElementList<TDerivedElement> derivedList)
            : base(derivedList)
        {
            _derivedList = derivedList ?? throw new ArgumentNullException(nameof(derivedList));
        }

        /// <summary>
        /// Occurs before the element list is moved.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> BeforeElementMoved
        {
            add => _derivedList.BeforeElementMoved += value;
            remove => _derivedList.BeforeElementMoved -= value;
        }

        /// <summary>
        /// Occurs after the element list is moved.
        /// </summary>
        public event EventHandler<ElementMovedEventArgs> AfterElementMoved
        {
            add => _derivedList.AfterElementMoved += value;
            remove => _derivedList.AfterElementMoved -= value;
        }

        /// <summary>
        /// Gets the underlying element list that stores derived elements.
        /// </summary>
        public new IElementList<TDerivedElement> DerivedList => _derivedList;

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="element">The element to insert.</param>
        public void Insert(int index, TBaseElement element)
        {
            if (element is TDerivedElement derivedElement)
            {
                _derivedList.Insert(index, derivedElement);
            }
            else
            {
                throw new ArgumentException($"Element must be of type {typeof(TDerivedElement).Name}", nameof(element));
            }
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _derivedList.RemoveAt(index);
        }

        public void Clear()
        {
            _derivedList.Clear();
        }
    }
}

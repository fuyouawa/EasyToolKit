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
    public class ElementListWrapper<TElement, TBaseElement> :
        ReadOnlyElementListWrapper<TElement, TBaseElement>,
        IElementListWrapper<TElement, TBaseElement>
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
            : base(baseList)
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
        /// Gets the underlying element list.
        /// </summary>
        public new IElementList<TBaseElement> BaseList => _baseList;

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

        public void Clear()
        {
            _baseList.Clear();
        }
    }
}

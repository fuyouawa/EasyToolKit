using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a mutable collection of elements in the inspector tree that supports adding, removing, and reordering.
    /// </summary>
    public interface IElementList<TElement> : IReadOnlyElementList<TElement>
        where TElement : IElement
    {
        /// <summary>
        /// Occurs before the element list is moved.
        /// </summary>
        event EventHandler<ElementMovedEventArgs> BeforeElementMoved;

        /// <summary>
        /// Occurs after the element list is moved.
        /// </summary>
        event EventHandler<ElementMovedEventArgs> AfterElementMoved;

        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="element">The element to insert.</param>
        void Insert(int index, TElement element);

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        void RemoveAt(int index);

        void Clear();
    }
}

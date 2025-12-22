using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a mutable collection of elements in the inspector tree that supports adding, removing, and reordering.
    /// </summary>
    public interface IElementList : IReadOnlyElementList
    {
        /// <summary>
        /// Inserts an element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="element">The element to insert.</param>
        void Insert(int index, IElement element);

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        void RemoveAt(int index);
    }
}

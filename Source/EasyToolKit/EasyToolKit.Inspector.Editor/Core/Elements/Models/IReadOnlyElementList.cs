using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a readonly collection of elements in the inspector tree.
    /// </summary>
    public interface IReadOnlyElementList<TElement> : IReadOnlyList<TElement>
        where TElement : IElement
    {
        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        TElement this[string name] { get; }

        IElement OwnerElement { get; }

        /// <summary>
        /// Gets the zero-based index of the first occurrence of an element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate.</param>
        /// <returns>The zero-based index of the first occurrence of the element, or -1 if not found.</returns>
        int IndexOf(string name);

        int IndexOf(TElement element);

        /// <summary>
        /// Gets the full path of the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The full path of the element.</returns>
        string GetPath(int index);
    }
}

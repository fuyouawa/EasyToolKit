using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection of elements in the inspector tree.
    /// </summary>
    public interface IElementCollection : IEnumerable<IElement>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        IElement this[int index] { get; }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element with the specified name.</returns>
        IElement this[string name] { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The element at the specified index.</returns>
        IElement Get(int index);

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>The element with the specified name, or null if no element exists with that name.</returns>
        IElement Get(string name);

        /// <summary>
        /// Gets the full path of the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        /// <returns>The full path of the element.</returns>
        string GetPath(int index);

        /// <summary>
        /// Recursively enumerates all elements and their descendants in the collection.
        /// </summary>
        /// <returns>An enumerable collection of all descendant elements.</returns>
        IEnumerable<IElement> Recurse();

        /// <summary>
        /// Updates the collection.
        /// </summary>
        void Update();

        /// <summary>
        /// Clears all cached elements in the collection.
        /// </summary>
        void Clear();
    }
}

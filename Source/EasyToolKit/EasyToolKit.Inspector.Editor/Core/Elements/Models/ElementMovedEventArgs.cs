using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the type of change that occurred to an element list.
    /// </summary>
    public enum ElementListChangeType
    {
        /// <summary>
        /// An element was inserted into the list.
        /// </summary>
        Insert,

        /// <summary>
        /// An element was removed from the list.
        /// </summary>
        Remove,
    }

    public class ElementMovedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementMovedEventArgs"/> class.
        /// </summary>
        /// <param name="changeType">The type of change that occurred.</param>
        /// <param name="index">The zero-based index at which the change occurred.</param>
        /// <param name="element">The element affected by the change.</param>
        /// <param name="oldParent">The previous parent element of the element.</param>
        /// <param name="newParent">The new parent element of the element (null if removed).</param>
        public ElementMovedEventArgs(ElementListChangeType changeType, int index, IElement element, IElement oldParent, IElement newParent)
        {
            ChangeType = changeType;
            Index = index;
            Element = element;
            OldParent = oldParent;
            NewParent = newParent;
        }

        /// <summary>
        /// Gets the type of change that occurred.
        /// </summary>
        public ElementListChangeType ChangeType { get; }

        /// <summary>
        /// Gets the zero-based index at which the change occurred.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the element affected by the change.
        /// </summary>
        public IElement Element { get; }

        /// <summary>
        /// Gets the previous parent element of the element.
        /// </summary>
        public IElement OldParent { get; }

        /// <summary>
        /// Gets the new parent element of the element (null if removed).
        /// </summary>
        public IElement NewParent { get; }
    }
}

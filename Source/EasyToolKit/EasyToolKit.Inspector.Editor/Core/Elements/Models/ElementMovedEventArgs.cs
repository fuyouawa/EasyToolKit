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

    /// <summary>
    /// Specifies the timing of the element moved event.
    /// </summary>
    public enum ElementMovedTiming
    {
        /// <summary>
        /// The event is raised before the element is moved.
        /// </summary>
        Before,

        /// <summary>
        /// The event is raised after the element is moved.
        /// </summary>
        After,
    }

    public class ElementMovedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementMovedEventArgs"/> class.
        /// </summary>
        /// <param name="changeType">The type of change that occurred.</param>
        /// <param name="element">The element that was moved.</param>
        /// <param name="index">The zero-based index at which the change occurred.</param>
        /// <param name="oldParent">The previous parent element of the element.</param>
        /// <param name="newParent">The new parent element of the element (null if removed).</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        public ElementMovedEventArgs(ElementListChangeType changeType, IElement element, int index, IElement oldParent, IElement newParent, ElementMovedTiming timing)
        {
            ChangeType = changeType;
            Element = element;
            Index = index;
            OldParent = oldParent;
            NewParent = newParent;
            Timing = timing;
        }

        /// <summary>
        /// Gets the type of change that occurred.
        /// </summary>
        public ElementListChangeType ChangeType { get; }

        /// <summary>
        /// Gets the element that was moved.
        /// </summary>
        public IElement Element { get; }

        /// <summary>
        /// Gets the zero-based index at which the change occurred.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the previous parent element of the element.
        /// </summary>
        public IElement OldParent { get; }

        /// <summary>
        /// Gets the new parent element of the element (null if removed).
        /// </summary>
        public IElement NewParent { get; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public ElementMovedTiming Timing { get; }
    }
}

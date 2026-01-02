using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

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

    [MustDisposeResource]
    public class ElementMovedEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Gets the type of change that occurred.
        /// </summary>
        public ElementListChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the element that was moved.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Gets the zero-based index at which the change occurred.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the previous parent element of the element.
        /// </summary>
        public IElement OldParent { get; private set; }

        /// <summary>
        /// Gets the new parent element of the element (null if removed).
        /// </summary>
        public IElement NewParent { get; private set; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public ElementMovedTiming Timing { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ElementMovedEventArgs"/> class from the object pool.
        /// </summary>
        /// <param name="changeType">The type of change that occurred.</param>
        /// <param name="element">The element that was moved.</param>
        /// <param name="index">The zero-based index at which the change occurred.</param>
        /// <param name="oldParent">The previous parent element of the element.</param>
        /// <param name="newParent">The new parent element of the element (null if removed).</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        /// <returns>A new or reused instance of <see cref="ElementMovedEventArgs"/>.</returns>
        public static ElementMovedEventArgs Create(ElementListChangeType changeType, IElement element, int index, IElement oldParent, IElement newParent, ElementMovedTiming timing)
        {
            var args = EditorPoolUtility.Rent<ElementMovedEventArgs>();
            args.ChangeType = changeType;
            args.Element = element;
            args.Index = index;
            args.OldParent = oldParent;
            args.NewParent = newParent;
            args.Timing = timing;
            return args;
        }

        /// <summary>
        /// Releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            EditorPoolUtility.Release(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
            ChangeType = default;
            Element = null;
            Index = 0;
            OldParent = null;
            NewParent = null;
            Timing = default;
        }
    }
}

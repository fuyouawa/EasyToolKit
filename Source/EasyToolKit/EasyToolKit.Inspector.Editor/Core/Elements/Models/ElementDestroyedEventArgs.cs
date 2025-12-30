using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides data for the element destroyed event.
    /// </summary>
    public class ElementDestroyedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDestroyedEventArgs"/> class.
        /// </summary>
        /// <param name="element">The element that was destroyed.</param>
        public ElementDestroyedEventArgs(IElement element)
        {
            Element = element;
        }

        /// <summary>
        /// Gets the element that was destroyed.
        /// </summary>
        public IElement Element { get; }
    }
}

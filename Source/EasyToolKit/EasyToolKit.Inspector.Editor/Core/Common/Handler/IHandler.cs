using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IHandler
    {
        /// <summary>
        /// Gets or sets the <see cref="IElement"/> that this drawer is associated with.
        /// </summary>
        IElement Element { get; set; }

        /// <summary>
        /// Determines whether this handler can process the specified element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this handler can handle the element; otherwise, false.</returns>
        bool CanHandle(IElement element);
    }
}

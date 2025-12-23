using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IHandler
    {
        /// <summary>
        /// Determines whether this handler can process the specified element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this handler can handle the element; otherwise, false.</returns>
        bool CanHandle(IElement element);
    }
}

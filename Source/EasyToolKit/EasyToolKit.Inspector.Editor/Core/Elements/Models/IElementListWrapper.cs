using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for element list wrappers.
    /// </summary>
    public interface IElementListWrapper
    {
    }

    /// <summary>
    /// Type-safe wrapper for an element list that exposes a more derived element type.
    /// </summary>
    /// <typeparam name="TElement">The derived element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseElement">The base element type stored in the underlying list.</typeparam>
    public interface IElementListWrapper<TElement, TBaseElement> : IElementList<TElement>, IElementListWrapper
        where TBaseElement : IElement
        where TElement : TBaseElement
    {
        /// <summary>
        /// Gets the underlying element list.
        /// </summary>
        IElementList<TBaseElement> BaseList { get; }
    }
}

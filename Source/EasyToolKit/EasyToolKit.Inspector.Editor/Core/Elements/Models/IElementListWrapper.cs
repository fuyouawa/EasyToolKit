namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for read-only element list wrappers.
    /// </summary>
    public interface IReadOnlyElementListWrapper
    {
    }

    /// <summary>
    /// Type-safe read-only wrapper for an element list that exposes a more derived element type.
    /// </summary>
    /// <typeparam name="TElement">The derived element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseElement">The base element type stored in the underlying list.</typeparam>
    public interface IReadOnlyElementListWrapper<TElement, TBaseElement> : IReadOnlyElementList<TElement>, IReadOnlyElementListWrapper
        where TBaseElement : IElement
        where TElement : TBaseElement
    {
        /// <summary>
        /// Gets the underlying element list.
        /// </summary>
        IReadOnlyElementList<TBaseElement> BaseList { get; }
    }

    /// <summary>
    /// Base interface for element list wrappers.
    /// </summary>
    public interface IElementListWrapper : IReadOnlyElementListWrapper
    {
    }

    /// <summary>
    /// Type-safe wrapper for an element list that exposes a more derived element type.
    /// </summary>
    /// <typeparam name="TElement">The derived element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseElement">The base element type stored in the underlying list.</typeparam>
    public interface IElementListWrapper<TElement, TBaseElement> : IElementList<TElement>, IReadOnlyElementListWrapper<TElement, TBaseElement>, IElementListWrapper
        where TBaseElement : IElement
        where TElement : TBaseElement
    {
        /// <summary>
        /// Gets the underlying element list.
        /// </summary>
        new IElementList<TBaseElement> BaseList { get; }
    }
}

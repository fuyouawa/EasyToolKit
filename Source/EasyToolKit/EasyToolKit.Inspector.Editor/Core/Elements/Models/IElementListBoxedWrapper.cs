namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Type-safe read-only wrapper for an element list that boxes derived elements to base type.
    /// This wrapper treats a list of derived elements as if they were base type elements.
    /// </summary>
    /// <typeparam name="TBaseElement">The base element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TDerivedElement">The derived element type stored in the underlying list.</typeparam>
    public interface IReadOnlyElementListBoxedWrapper<TBaseElement, TDerivedElement> : IReadOnlyElementList<TBaseElement>
        where TBaseElement : IElement
        where TDerivedElement : TBaseElement
    {
        /// <summary>
        /// Gets the underlying element list that stores derived elements.
        /// </summary>
        IReadOnlyElementList<TDerivedElement> DerivedList { get; }
    }

    /// <summary>
    /// Type-safe wrapper for an element list that boxes derived elements to base type.
    /// This wrapper treats a list of derived elements as if they were base type elements.
    /// </summary>
    /// <typeparam name="TBaseElement">The base element type exposed by this wrapper.</typeparam>
    /// <typeparam name="TDerivedElement">The derived element type stored in the underlying list.</typeparam>
    public interface IElementListBoxedWrapper<TBaseElement, TDerivedElement> : IElementList<TBaseElement>, IReadOnlyElementListBoxedWrapper<TBaseElement, TDerivedElement>
        where TBaseElement : IElement
        where TDerivedElement : TBaseElement
    {
        /// <summary>
        /// Gets the underlying element list that stores derived elements.
        /// </summary>
        new IElementList<TDerivedElement> DerivedList { get; }
    }
}

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Root element interface for the inspector tree.
    /// An abstract concept similar to dynamically created values, representing the root Unity instance being inspected.
    /// </summary>
    public interface IRootElement : IValueElement
    {
        /// <summary>
        /// Gets the root definition that describes this root element.
        /// </summary>
        new IRootDefinition Definition { get; }
    }
}

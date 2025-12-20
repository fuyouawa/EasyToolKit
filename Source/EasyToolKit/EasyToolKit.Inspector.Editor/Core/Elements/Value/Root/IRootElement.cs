namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the root element of an inspector tree.
    /// </summary>
    public interface IRootElement : IValueElement
    {
        /// <summary>
        /// Gets the root definition that describes this root element.
        /// </summary>
        new IRootDefinition Definition { get; }
    }
}

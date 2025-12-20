namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a method element in the inspector tree.
    /// </summary>
    public interface IMethodElement : IElement
    {
        /// <summary>
        /// Gets the method definition that describes this method.
        /// </summary>
        new IMethodDefinition Definition { get; }

        /// <summary>
        /// Gets the collection of parameter elements for this method.
        /// </summary>
        IElementCollection Parameters { get; }
    }
}

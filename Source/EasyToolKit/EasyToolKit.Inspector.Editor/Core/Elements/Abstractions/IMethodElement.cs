namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Method element interface for function handling in the inspector tree.
    /// Represents methods that can be invoked with parameters and displayed in the inspector.
    /// </summary>
    public interface IMethodElement : ILogicalElement
    {
        /// <summary>
        /// Gets the method definition that describes this method.
        /// </summary>
        new IMethodDefinition Definition { get; }

        /// <summary>
        /// Gets the collection of parameter elements for this method.
        /// </summary>
        new IReadOnlyElementList<IMethodParameterElement> LogicalChildren { get; }
    }
}

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Method parameter element interface representing individual parameters in methods.
    /// As an abstract parameter element concept, supports parameter value handling for method invocation.
    /// </summary>
    public interface IMethodParameterElement : IValueElement
    {
        /// <summary>
        /// Gets the method parameter definition that describes this parameter.
        /// </summary>
        new IMethodParameterDefinition Definition { get; }

        /// <summary>
        /// Gets the logical parent method element that contains this parameter.
        /// </summary>
        new IMethodElement LogicalParent { get; }
    }
}

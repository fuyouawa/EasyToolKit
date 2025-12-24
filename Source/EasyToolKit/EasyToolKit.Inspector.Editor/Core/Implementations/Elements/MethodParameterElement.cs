using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method parameter element implementation representing individual parameters in methods.
    /// As an abstract parameter element concept, supports parameter value handling for method invocation.
    /// </summary>
    public class MethodParameterElement : ValueElement, IMethodParameterElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameterElement"/> class.
        /// </summary>
        /// <param name="definition">The method parameter definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent method element that contains this parameter.</param>
        public MethodParameterElement(
            [NotNull] IMethodParameterDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IMethodElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the method parameter definition that describes this parameter.
        /// </summary>
        public new IMethodParameterDefinition Definition => (IMethodParameterDefinition)base.Definition;

        /// <summary>
        /// Gets the logical parent method element that contains this parameter.
        /// </summary>
        public new IMethodElement LogicalParent => (IMethodElement)base.LogicalParent;
    }
}

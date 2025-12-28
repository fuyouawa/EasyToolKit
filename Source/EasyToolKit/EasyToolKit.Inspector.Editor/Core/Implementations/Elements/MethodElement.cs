using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method element implementation for function handling in the inspector tree.
    /// Represents methods that can be invoked with parameters and displayed in the inspector.
    /// </summary>
    public class MethodElement : LogicalElementBase, IMethodElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodElement"/> class.
        /// </summary>
        /// <param name="definition">The method definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public MethodElement(
            [NotNull] IMethodDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] ILogicalElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the method definition that describes this method.
        /// </summary>
        public new IMethodDefinition Definition => (IMethodDefinition)base.Definition;

        /// <summary>
        /// Gets the collection of parameter elements for this method.
        /// </summary>
        public new IReadOnlyElementList<IMethodParameterElement> LogicalChildren =>
            ((IReadOnlyElementListBoxedWrapper<ILogicalElement, IMethodParameterElement>)base.LogicalChildren)!.DerivedList;

        /// <summary>
        /// Determines whether this element can have children.
        /// Methods always have parameter elements as children.
        /// </summary>
        protected override bool CanHaveChildren()
        {
            return true;
        }

        /// <summary>
        /// Creates the logical children list based on the structure resolver.
        /// Wraps the base element list to expose strongly-typed parameter elements.
        /// </summary>
        /// <returns>A read-only list of parameter elements for this method.</returns>
        protected override IReadOnlyElementList<ILogicalElement> CreateLogicalChildren()
        {
            var baseLogicalChildren = base.CreateLogicalChildren();
            var wrapper = new ReadOnlyElementListWrapper<IMethodParameterElement, ILogicalElement>(baseLogicalChildren);
            return new ReadOnlyElementListBoxedWrapper<ILogicalElement, IMethodParameterElement>(wrapper);
        }
    }
}

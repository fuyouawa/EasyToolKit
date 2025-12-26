using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Root element implementation for the inspector tree.
    /// An abstract concept similar to dynamically created values, representing the root Unity instance being inspected.
    /// </summary>
    public class RootElement : ValueElement, IRootElement
    {
        /// <summary>
        /// Gets the root definition that describes this root element.
        /// </summary>
        public new IRootDefinition Definition => (IRootDefinition)base.Definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootElement"/> class.
        /// </summary>
        /// <param name="definition">The root definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        public RootElement(
            [NotNull] IRootDefinition definition,
            [NotNull] IElementSharedContext sharedContext)
            : base(definition, sharedContext, null)
        {
        }

        /// <summary>
        /// Gets the hierarchical path of this element.
        /// </summary>
        public override string Path => Definition.Name;

        protected override bool CanHaveChildren()
        {
            return true;
        }
    }
}

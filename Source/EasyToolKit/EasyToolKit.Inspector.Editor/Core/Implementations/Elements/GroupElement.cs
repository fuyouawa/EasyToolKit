using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Group element implementation for organizing elements in the inspector tree.
    /// Groups are abstract concepts for organizing elements, controlled by <see cref="BeginGroupAttribute"/>
    /// (e.g., <see cref="FoldoutGroupAttribute"/>) and <see cref="EndGroupAttribute"/>
    /// (e.g., <see cref="EndFoldoutGroupAttribute"/>).
    /// Groups can also be automatically closed when a subsequent group's path is not a sub‑path of
    /// the current group (e.g., current group "A/B", next group "C/D" or "A/C").
    /// </summary>
    public class GroupElement : ElementBase, IGroupElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupElement"/> class.
        /// </summary>
        /// <param name="definition">The group definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public GroupElement(
            [NotNull] IGroupDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the group definition that describes this group.
        /// </summary>
        public new IGroupDefinition Definition => (IGroupDefinition)base.Definition;
    }
}

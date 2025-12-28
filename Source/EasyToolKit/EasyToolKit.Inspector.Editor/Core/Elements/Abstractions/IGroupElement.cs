using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Group element interface for organizing elements in the inspector tree.
    /// Groups are abstract concepts for organizing elements, controlled by <see cref="BeginGroupAttribute"/> (e.g., <see cref="FoldoutGroupAttribute"/>) and <see cref="EndGroupAttribute"/> (e.g., <see cref="EndFoldoutGroupAttribute"/>).
    /// Groups can also be automatically closed when a subsequent group's path is not a sub‑path of the current group (e.g., current group "A/B", next group "C/D" or "A/C").
    /// </summary>
    public interface IGroupElement : IElement
    {
        /// <summary>
        /// Gets the group definition that describes this group.
        /// </summary>
        new IGroupDefinition Definition { get; }

        [CanBeNull] ILogicalElement AssociatedElement { get; set; }
    }
}

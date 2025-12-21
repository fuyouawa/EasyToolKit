using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Group definition interface for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public interface IGroupDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the type of the attribute that begins this group (e.g., <see cref="FoldoutGroupAttribute"/>).
        /// </summary>
        Type BeginGroupAttributeType { get; }

        /// <summary>
        /// Gets the type of the attribute that ends this group (e.g., <see cref="EndFoldoutGroupAttribute"/>).
        /// </summary>
        Type EndGroupAttributeType { get; }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a group definition in the inspector.
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

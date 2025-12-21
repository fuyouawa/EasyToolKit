using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating group element definitions.
    /// Groups organize related inspector elements using begin/end attribute pairs.
    /// </summary>
    public interface IGroupConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the type of attribute that begins this group (e.g., FoldoutGroupAttribute).
        /// This attribute marks the start of a logical grouping in the inspector.
        /// </summary>
        Type BeginGroupAttributeType { get; set; }

        /// <summary>
        /// Gets or sets the type of attribute that ends this group (e.g., EndFoldoutGroupAttribute).
        /// This attribute marks the end of the logical grouping.
        /// </summary>
        Type EndGroupAttributeType { get; set; }

        /// <summary>
        /// Creates a new <see cref="IGroupDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new group definition instance.</returns>
        IGroupDefinition CreateDefinition();
    }
}
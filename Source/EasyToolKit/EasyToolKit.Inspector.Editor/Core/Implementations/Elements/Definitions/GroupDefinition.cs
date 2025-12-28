using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Group definition implementation for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public sealed class GroupDefinition : ElementDefinition, IGroupDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="beginGroupAttributeType">The type of the attribute that begins this group.</param>
        /// <param name="endGroupAttributeType">The type of the attribute that ends this group.</param>
        public GroupDefinition(ElementRoles roles, string name, Type beginGroupAttributeType, Type endGroupAttributeType)
            : base(roles, name)
        {
            BeginGroupAttributeType = beginGroupAttributeType;
            EndGroupAttributeType = endGroupAttributeType;
        }

        /// <summary>
        /// Gets the type of the attribute that begins this group (e.g., <see cref="FoldoutGroupAttribute"/>).
        /// </summary>
        public Type BeginGroupAttributeType { get; }

        /// <summary>
        /// Gets the type of the attribute that ends this group (e.g., <see cref="EndFoldoutGroupAttribute"/>).
        /// </summary>
        public Type EndGroupAttributeType { get; }
    }
}

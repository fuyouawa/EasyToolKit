using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Group definition implementation for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public sealed class GroupDefinition : ElementDefinition, IGroupDefinition
    {
        /// <summary>
        /// Gets or sets the type of the attribute that begins this group (e.g., <see cref="FoldoutGroupAttribute"/>).
        /// </summary>
        public Type BeginGroupAttributeType { get; set; }

        /// <summary>
        /// Gets or sets the type of the attribute that ends this group (e.g., <see cref="EndFoldoutGroupAttribute"/>).
        /// </summary>
        public Type EndGroupAttributeType { get; set; }
    }
}

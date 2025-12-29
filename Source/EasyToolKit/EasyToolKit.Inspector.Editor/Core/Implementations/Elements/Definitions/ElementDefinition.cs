using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents the definition of an element in the inspector hierarchy.
    /// It serves as the base interface for all element definitions.
    /// </summary>
    public abstract class ElementDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets or sets the flags of the element.
        /// </summary>
        public ElementRoles Roles { get; set; }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the additional attributes for customizing the element behavior.
        /// </summary>
        public IReadOnlyList<Attribute> AdditionalAttributes { get; set; }
    }
}

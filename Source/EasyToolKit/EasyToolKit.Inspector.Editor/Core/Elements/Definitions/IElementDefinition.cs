using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the definition of an element in the inspector hierarchy.
    /// It serves as the base interface for all element definitions.
    /// </summary>
    public interface IElementDefinition
    {
        /// <summary>
        /// Gets the flags of the element.
        /// </summary>
        ElementRoles Roles { get; }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the additional attributes for customizing the element behavior.
        /// </summary>
        [CanBeNull] IReadOnlyList<Attribute> AdditionalAttributes { get; }
    }
}

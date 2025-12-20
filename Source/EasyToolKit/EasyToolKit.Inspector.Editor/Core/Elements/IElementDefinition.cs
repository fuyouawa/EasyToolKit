using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the definition of an element in the inspector hierarchy.
    /// It serves as the base interface for all element definitions.
    /// </summary>
    public interface IElementDefinition
    {
        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of the owner (parent) that contains this element.
        /// </summary>
        Type OwnerType { get; }
    }
}

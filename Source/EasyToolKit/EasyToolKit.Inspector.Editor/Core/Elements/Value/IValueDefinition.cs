using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a value definition in the inspector.
    /// </summary>
    public interface IValueDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        Type ValueType { get; }
    }
}

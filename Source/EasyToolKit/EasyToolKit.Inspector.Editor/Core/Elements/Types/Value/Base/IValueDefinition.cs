using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base value definition interface for all data-containing elements.
    /// Derives <see cref="IPropertyDefinition"/> and <see cref="ICollectionItemDefinition"/>,
    /// and can also be created independently to represent dynamic data.
    /// </summary>
    public interface IValueDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        Type ValueType { get; }
    }
}

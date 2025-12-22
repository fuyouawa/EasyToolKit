using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating value element definitions.
    /// Values represent fields, properties, or dynamically created custom values.
    /// </summary>
    public interface IValueConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the value element.
        /// This determines the data type that the element will hold or manipulate.
        /// </summary>
        Type ValueType { get; set; }

        /// <summary>
        /// Creates a new <see cref="IValueDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new value definition instance.</returns>
        IValueDefinition CreateDefinition();
    }
}

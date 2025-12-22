using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating property element definitions.
    /// Properties provide consistent access to property member data.
    /// </summary>
    public interface IPropertyConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// Provides access to reflection information about the underlying property.
        /// </summary>
        PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="IPropertyDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property definition instance.</returns>
        new IPropertyDefinition CreateDefinition();
    }
}
using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating property element definitions.
    /// Properties provide consistent access to property member data.
    /// </summary>
    public class PropertyConfiguration : ValueConfiguration, IPropertyConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// Provides access to reflection information about the underlying property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="IPropertyDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property definition instance.</returns>
        public new IPropertyDefinition CreateDefinition()
        {
            if (PropertyInfo == null)
            {
                throw new InvalidOperationException("PropertyInfo cannot be null");
            }

            if (Name.IsNotNullOrWhiteSpace())
            {
                Name = PropertyInfo.Name;
            }

            return new PropertyDefinition(ElementFlags.Property | ElementFlags.Value, Name, PropertyInfo);
        }
    }
}

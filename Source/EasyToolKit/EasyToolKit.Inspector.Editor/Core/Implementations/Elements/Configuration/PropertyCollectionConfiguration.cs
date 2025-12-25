using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating property collection element definitions.
    /// Property collections represent collection properties on an object.
    /// </summary>
    public class PropertyCollectionConfiguration : CollectionConfiguration, IPropertyCollectionConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="IPropertyCollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property collection definition instance.</returns>
        public new IPropertyCollectionDefinition CreateDefinition()
        {
            if (PropertyInfo == null)
            {
                throw new InvalidOperationException("PropertyInfo cannot be null");
            }

            if (ItemType == null)
            {
                throw new InvalidOperationException("ItemType cannot be null");
            }

            if (Name.IsNotNullOrWhiteSpace())
            {
                Name = PropertyInfo.Name;
            }

            return new PropertyCollectionDefinition(ElementFlags.Property | ElementFlags.Collection | ElementFlags.Value, Name, PropertyInfo,
                ItemType);
        }

        IPropertyDefinition IPropertyConfiguration.CreateDefinition()
        {
            return CreateDefinition();
        }
    }
}

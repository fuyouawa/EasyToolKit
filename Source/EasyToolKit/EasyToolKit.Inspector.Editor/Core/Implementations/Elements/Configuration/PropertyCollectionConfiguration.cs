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

        protected void ProcessDefinition(PropertyCollectionDefinition definition)
        {
            if (PropertyInfo == null)
            {
                throw new InvalidOperationException("PropertyInfo cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                Name = PropertyInfo.Name;
            }

            ValueType = PropertyInfo.PropertyType;

            definition.Roles = definition.Roles.Add(ElementRoles.Property);
            definition.PropertyInfo = PropertyInfo;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IPropertyCollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property collection definition instance.</returns>
        public new IPropertyCollectionDefinition CreateDefinition()
        {
            var definition = new PropertyCollectionDefinition();
            ProcessDefinition(definition);
            return definition;
        }

        IPropertyDefinition IPropertyConfiguration.CreateDefinition()
        {
            return CreateDefinition();
        }
    }
}

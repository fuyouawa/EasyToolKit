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

        protected void ProcessDefinition(PropertyDefinition definition)
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
        /// Creates a new <see cref="IPropertyDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property definition instance.</returns>
        public new IPropertyDefinition CreateDefinition()
        {
            var definition = new PropertyDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

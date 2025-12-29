using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating field collection element definitions.
    /// Field collections represent collection fields on an object.
    /// </summary>
    public class FieldCollectionConfiguration : CollectionConfiguration, IFieldCollectionConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// </summary>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// Gets or sets whether this field should be rendered using Unity's built-in property drawer
        /// instead of the framework's custom <see cref="EasyDrawer"/>.
        /// </summary>
        public bool AsUnityProperty { get; set; }

        protected void ProcessDefinition(FieldCollectionDefinition definition)
        {
            if (FieldInfo == null)
            {
                throw new InvalidOperationException("FieldInfo cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                Name = FieldInfo.Name;
            }

            ValueType = FieldInfo.FieldType;

            definition.Roles = definition.Roles.Add(ElementRoles.Field);
            definition.FieldInfo = FieldInfo;
            definition.AsUnityProperty = AsUnityProperty;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IFieldCollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new field collection definition instance.</returns>
        public new IFieldCollectionDefinition CreateDefinition()
        {
            var definition = new FieldCollectionDefinition();
            ProcessDefinition(definition);
            return definition;
        }

        IFieldDefinition IFieldConfiguration.CreateDefinition()
        {
            return CreateDefinition();
        }
    }
}

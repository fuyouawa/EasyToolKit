using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating field element definitions.
    /// Fields provide consistent access to field member data.
    /// </summary>
    public class FieldConfiguration : ValueConfiguration, IFieldConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// Provides access to reflection information about the underlying field.
        /// </summary>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// Gets or sets whether this field should be rendered using Unity's built-in property drawer
        /// instead of the framework's custom <see cref="EasyDrawer"/>.
        /// When true, Unity handles the rendering; when false, the framework provides enhanced rendering.
        /// </summary>
        public bool AsUnityProperty { get; set; }

        protected void ProcessDefinition(FieldDefinition definition)
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
        /// Creates a new <see cref="IFieldDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new field definition instance.</returns>
        public new IFieldDefinition CreateDefinition()
        {
            var definition = new FieldDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

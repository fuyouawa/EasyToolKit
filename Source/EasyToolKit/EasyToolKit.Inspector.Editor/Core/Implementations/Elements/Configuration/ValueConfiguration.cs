using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating value element definitions.
    /// Values represent fields, properties, or dynamically created custom values.
    /// </summary>
    public class ValueConfiguration : ElementConfiguration, IValueConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the value element.
        /// This determines the data type that the element will hold or manipulate.
        /// </summary>
        public Type ValueType { get; set; }

        protected void ProcessDefinition(ValueDefinition definition)
        {
            if (ValueType == null)
            {
                throw new InvalidOperationException("ValueType cannot be null");
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Value);
            definition.ValueType = ValueType;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IValueDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new value definition instance.</returns>
        public IValueDefinition CreateDefinition()
        {
            var definition = new ValueDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating group element definitions.
    /// Groups define logical sections in the inspector interface.
    /// </summary>
    public class GroupConfiguration : ElementConfiguration, IGroupConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the attribute that begins this group
        /// (e.g., <see cref="FoldoutGroupAttribute"/>).
        /// </summary>
        public Type BeginGroupAttributeType { get; set; }

        /// <summary>
        /// Gets or sets the type of the attribute that ends this group
        /// (e.g., <see cref="EndFoldoutGroupAttribute"/>).
        /// </summary>
        public Type EndGroupAttributeType { get; set; }

        protected void ProcessDefinition(GroupDefinition definition)
        {
            if (BeginGroupAttributeType == null)
            {
                throw new InvalidOperationException("BeginGroupAttributeType cannot be null");
            }

            if (EndGroupAttributeType == null)
            {
                throw new InvalidOperationException("EndGroupAttributeType cannot be null");
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Group);
            definition.BeginGroupAttributeType = BeginGroupAttributeType;
            definition.EndGroupAttributeType = EndGroupAttributeType;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IGroupDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new group definition instance.</returns>
        public IGroupDefinition CreateDefinition()
        {
            var definition = new GroupDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating root element definitions.
    /// The root represents the Unity instance being inspected.
    /// </summary>
    public class RootConfiguration : ValueConfiguration, IRootConfiguration
    {
        protected void ProcessDefinition(RootDefinition definition)
        {
            if (Name.IsNullOrWhiteSpace())
            {
                Name = "$Root$";
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Root);
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IRootDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new root definition instance.</returns>
        public new IRootDefinition CreateDefinition()
        {
            var definition = new RootDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

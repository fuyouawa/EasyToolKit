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
        /// <summary>
        /// Creates a new <see cref="IRootDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new root definition instance.</returns>
        public new IRootDefinition CreateDefinition()
        {
            if (ValueType == null)
            {
                throw new InvalidOperationException("ValueType cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                Name = "$Root$";
            }

            return new RootDefinition(ElementRoles.Root | ElementRoles.Value, Name, ValueType);
        }
    }
}

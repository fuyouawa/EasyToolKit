using System;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Root definition implementation for the inspector tree.
    /// An abstract concept similar to dynamically created values, used to represent Unity instances.
    /// </summary>
    public sealed class RootDefinition : ValueDefinition, IRootDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="valueType">The type of the value.</param>
        public RootDefinition(ElementRoles roles, string name, Type valueType)
            : base(roles, name, valueType)
        {
        }
    }
}

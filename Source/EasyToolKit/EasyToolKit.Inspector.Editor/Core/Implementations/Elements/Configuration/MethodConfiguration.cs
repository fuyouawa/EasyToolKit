using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating method element definitions.
    /// Methods represent functions that can be invoked or displayed in the inspector.
    /// </summary>
    public class MethodConfiguration : ElementConfiguration, IMethodConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.MethodInfo"/> that represents this method.
        /// Provides access to reflection information about the underlying method.
        /// </summary>
        public MethodInfo MethodInfo { get; set; }

        protected void ProcessDefinition(MethodDefinition definition)
        {
            if (MethodInfo == null)
            {
                throw new InvalidOperationException("MethodInfo cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                Name = MethodInfo.Name;
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Method);
            definition.MethodInfo = MethodInfo;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IMethodDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new method definition instance.</returns>
        public IMethodDefinition CreateDefinition()
        {
            var definition = new MethodDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

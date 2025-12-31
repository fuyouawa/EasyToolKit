using System;
using System.Reflection;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating method parameter element definitions.
    /// Method parameters represent individual parameters for method invocation.
    /// </summary>
    public class MethodParameterConfiguration : ValueConfiguration, IMethodParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.ParameterInfo"/> that describes this parameter.
        /// </summary>
        public ParameterInfo ParameterInfo { get; set; }

        /// <summary>
        /// Gets or sets the index of this parameter in the method's parameter list.
        /// </summary>
        public int ParameterIndex { get; set; }

        protected void ProcessDefinition(MethodParameterDefinition definition)
        {
            if (ParameterInfo == null)
            {
                throw new InvalidOperationException("ParameterInfo cannot be null");
            }

            if (Name.IsNullOrWhiteSpace())
            {
                Name = ParameterInfo.Name;
            }
            ValueType = ParameterInfo.ParameterType;

            definition.Roles = definition.Roles.Add(ElementRoles.MethodParameter);
            definition.ParameterInfo = ParameterInfo;
            definition.ParameterIndex = ParameterIndex;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IMethodParameterDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new method parameter definition instance.</returns>
        public new IMethodParameterDefinition CreateDefinition()
        {
            var definition = new MethodParameterDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}

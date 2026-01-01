using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating method parameter element definitions.
    /// Method parameters represent individual parameter values for method invocation.
    /// </summary>
    public interface IMethodParameterConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Gets or sets the index of this parameter in the method's parameter list.
        /// This determines the position of the parameter when invoking the method.
        /// </summary>
        int ParameterIndex { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterInfo"/> that describes the method parameter.
        /// This contains metadata about the parameter including type, default value, and attributes.
        /// </summary>
        ParameterInfo ParameterInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="IMethodParameterDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new method parameter definition instance.</returns>
        new IMethodParameterDefinition CreateDefinition();
    }
}

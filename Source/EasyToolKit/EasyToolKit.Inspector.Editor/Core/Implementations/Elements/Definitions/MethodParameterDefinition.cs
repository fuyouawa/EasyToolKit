using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method parameter definition implementation handling individual parameters for method invocation.
    /// Similar to dynamically created custom values, representing individual parameter items for method invocation.
    /// </summary>
    public sealed class MethodParameterDefinition : ValueDefinition, IMethodParameterDefinition
    {
        /// <summary>
        /// Gets or sets the index of this parameter in the method's parameter list.
        /// </summary>
        public int ParameterIndex { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ParameterInfo"/> that describes the method parameter.
        /// </summary>
        public ParameterInfo ParameterInfo { get; set; }
    }
}

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
        /// Initializes a new instance of the <see cref="MethodParameterDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="parameterInfo">The parameter information.</param>
        /// <param name="parameterIndex">The index of this parameter in the method's parameter list.</param>
        public MethodParameterDefinition(ElementRoles roles, string name, ParameterInfo parameterInfo, int parameterIndex)
            : base(roles, name, parameterInfo.ParameterType)
        {
            ParameterInfo = parameterInfo;
            ParameterIndex = parameterIndex;
        }

        /// <summary>
        /// Gets the index of this parameter in the method's parameter list.
        /// </summary>
        public int ParameterIndex { get; }

        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> that describes the method parameter.
        /// </summary>
        public ParameterInfo ParameterInfo { get; }
    }
}

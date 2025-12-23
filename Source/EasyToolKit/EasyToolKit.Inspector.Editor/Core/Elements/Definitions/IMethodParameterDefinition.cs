using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Method parameter definition interface handling the abstract concept of method parameters.
    /// Similar to dynamically created custom values, representing individual parameter items for method invocation.
    /// </summary>
    public interface IMethodParameterDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets the index of this parameter in the method's parameter list.
        /// </summary>
        int ParameterIndex { get; }

        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> that describes the method parameter.
        /// </summary>
        ParameterInfo ParameterInfo { get; }
    }
}

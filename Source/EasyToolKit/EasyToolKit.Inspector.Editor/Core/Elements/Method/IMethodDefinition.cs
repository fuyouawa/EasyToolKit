using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a method definition in the inspector.
    /// </summary>
    public interface IMethodDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that describes the method.
        /// </summary>
        MethodInfo MethodInfo { get; }
    }
}

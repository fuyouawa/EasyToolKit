using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Method definition interface for function handling in the inspector.
    /// Provides metadata for methods that can be invoked or displayed in the inspector interface.
    /// </summary>
    public interface IMethodDefinition : IElementDefinition, IMemberDefinition
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that describes the method.
        /// </summary>
        MethodInfo MethodInfo { get; }
    }
}

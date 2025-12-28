using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method definition implementation for function handling in the inspector.
    /// Provides metadata for methods that can be invoked or displayed in the inspector interface.
    /// </summary>
    public sealed class MethodDefinition : ElementDefinition, IMethodDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDefinition"/> class.
        /// </summary>
        /// <param name="roles">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="methodInfo">The method information.</param>
        public MethodDefinition(ElementRoles roles, string name, MethodInfo methodInfo)
            : base(roles, name)
        {
            MethodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that describes the method.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        public MemberInfo MemberInfo => MethodInfo;
    }
}

using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="IMemberDefinition"/> for members with reflection information.
    /// </summary>
    public abstract class MemberDefinition : IMemberDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDefinition"/> class.
        /// </summary>
        /// <param name="memberInfo">The reflection information about the member.</param>
        protected MemberDefinition(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }

        /// <summary>
        /// Gets the reflection information about the member.
        /// </summary>
        public MemberInfo MemberInfo { get; }
    }
}

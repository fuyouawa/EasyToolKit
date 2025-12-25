using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property definition implementation that handles <see cref="System.Reflection.PropertyInfo"/>.
    /// Provides consistent access to property reflection information.
    /// </summary>
    public sealed class PropertyDefinition : ValueDefinition, IPropertyDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="propertyInfo">The property information.</param>
        public PropertyDefinition(ElementFlags flags, string name, PropertyInfo propertyInfo)
            : base(flags, name, propertyInfo.PropertyType)
        {
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        public MemberInfo MemberInfo => PropertyInfo;
    }
}

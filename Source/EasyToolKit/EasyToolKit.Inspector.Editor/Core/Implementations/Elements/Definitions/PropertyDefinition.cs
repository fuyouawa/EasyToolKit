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
        /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => PropertyInfo;
    }
}

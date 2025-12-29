using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field definition implementation that handles <see cref="System.Reflection.FieldInfo"/>.
    /// Provides consistent access to field reflection information.
    /// </summary>
    public sealed class FieldDefinition : ValueDefinition, IFieldDefinition
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// </summary>
        public FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// Gets or sets whether this field should be treated as a Unity property.
        /// </summary>
        public bool AsUnityProperty { get; set; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => FieldInfo;
    }
}

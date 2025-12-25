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
        /// Initializes a new instance of the <see cref="FieldDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="asUnityProperty">Whether this field should be treated as a Unity property.</param>
        public FieldDefinition(ElementFlags flags, string name, FieldInfo fieldInfo, bool asUnityProperty)
            : base(flags, name, fieldInfo.FieldType)
        {
            FieldInfo = fieldInfo;
            AsUnityProperty = asUnityProperty;
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets whether this field should be treated as a Unity property.
        /// </summary>
        public bool AsUnityProperty { get; }

        public MemberInfo MemberInfo => FieldInfo;
    }
}

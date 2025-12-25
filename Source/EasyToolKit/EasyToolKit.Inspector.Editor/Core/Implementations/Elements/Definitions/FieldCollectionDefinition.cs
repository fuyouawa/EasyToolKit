using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field collection definition implementation that unifies <see cref="ICollectionDefinition"/> and <see cref="IFieldDefinition"/>.
    /// Represents collection fields on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public sealed class FieldCollectionDefinition : CollectionDefinition, IFieldCollectionDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCollectionDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="asUnityProperty">Whether this field should be treated as a Unity property.</param>
        public FieldCollectionDefinition(ElementFlags flags, string name, FieldInfo fieldInfo, bool asUnityProperty, System.Type itemType)
            : base(flags, name, fieldInfo.FieldType, itemType)
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

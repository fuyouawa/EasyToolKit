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

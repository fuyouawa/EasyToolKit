using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Field definition interface that handles <see cref="System.Reflection.FieldInfo"/>.
    /// Provides consistent access to field reflection information.
    /// </summary>
    public interface IFieldDefinition : IValueDefinition, IMemberDefinition
    {
        /// <summary>
        /// Gets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// Provides access to reflection information about the underlying field.
        /// </summary>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets whether this field should be treated as a Unity property, using Unity's built‑in drawer instead of the framework's <see cref="EasyDrawer"/>.
        /// </summary>
        bool AsUnityProperty { get; }
    }
}

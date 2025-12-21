using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property definition interface that unifies <see cref="System.Reflection.PropertyInfo"/> and <see cref="System.Reflection.FieldInfo"/>.
    /// Acts as a union type for properties and fields, providing a consistent access interface.
    /// </summary>
    public interface IPropertyDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets whether this property should be treated as a Unity property, using Unity's built‑in drawer instead of the framework's <see cref="EasyDrawer"/>.
        /// </summary>
        bool AsUnityProperty { get; }
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a property definition in the inspector.
    /// </summary>
    public interface IPropertyDefinition : IValueDefinition
    {
        /// <summary>
        /// Gets whether this property should be treated as a Unity property, using Unity's built‑in drawer instead of the framework's <see cref="EasyDrawer"/>.
        /// </summary>
        bool AsUnityProperty { get; }
    }
}

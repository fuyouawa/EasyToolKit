using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating property element definitions.
    /// Properties unify fields and properties, providing consistent access to member data.
    /// </summary>
    public interface IPropertyConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Gets or sets whether this property should be rendered using Unity's built-in property drawer
        /// instead of the framework's custom <see cref="EasyDrawer"/>.
        /// When true, Unity handles the rendering; when false, the framework provides enhanced rendering.
        /// </summary>
        bool AsUnityProperty { get; set; }

        /// <summary>
        /// Creates a new <see cref="IPropertyDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property definition instance.</returns>
        new IPropertyDefinition CreateDefinition();
    }
}
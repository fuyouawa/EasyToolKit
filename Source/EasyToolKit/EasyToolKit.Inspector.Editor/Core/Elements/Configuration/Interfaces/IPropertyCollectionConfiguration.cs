using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating property collection element definitions.
    /// Combines the collection configuration of <see cref="ICollectionConfiguration"/> with the property-specific features of <see cref="IPropertyConfiguration"/>.
    /// Represents collection properties on an object.
    /// </summary>
    public interface IPropertyCollectionConfiguration : ICollectionConfiguration, IPropertyConfiguration
    {
        /// <summary>
        /// Creates a new <see cref="IPropertyCollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new property collection definition instance.</returns>
        new IPropertyCollectionDefinition CreateDefinition();
    }
}
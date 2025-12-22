namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating field collection element definitions.
    /// Combines the collection configuration of <see cref="ICollectionConfiguration"/> with the field-specific features of <see cref="IFieldConfiguration"/>.
    /// Represents collection fields on an object.
    /// </summary>
    public interface IFieldCollectionConfiguration : ICollectionConfiguration, IFieldConfiguration
    {
        /// <summary>
        /// Creates a new <see cref="IFieldCollectionDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new field collection definition instance.</returns>
        new IFieldCollectionDefinition CreateDefinition();
    }
}
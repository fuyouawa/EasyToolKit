namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for locating property resolvers based on inspector property characteristics.
    /// Implementations determine the appropriate resolver for handling property serialization and display.
    /// </summary>
    public interface IPropertyStructureResolverLocator
    {
        bool CanResolver(InspectorProperty property);

        /// <summary>
        /// Gets the appropriate property resolver for the specified inspector property.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>An IPropertyResolver instance suitable for handling the property.</returns>
        IPropertyStructureResolver GetResolver(InspectorProperty property);
    }
}

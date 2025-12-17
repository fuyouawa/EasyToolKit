namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for locating property resolvers based on inspector property characteristics.
    /// Implementations determine the appropriate resolver for handling property serialization and display.
    /// </summary>
    public interface IPropertyResolverLocator
    {
        /// <summary>
        /// Gets the appropriate property resolver for the specified inspector property.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>An IPropertyResolver instance suitable for handling the property.</returns>
        IPropertyStructureResolver GetResolver(InspectorProperty property);
    }

    /// <summary>
    /// Abstract base class for property resolver locators that provides a foundation
    /// for implementing specific resolver location strategies.
    /// </summary>
    public abstract class PropertyResolverLocator : IPropertyResolverLocator
    {
        /// <summary>
        /// When implemented in a derived class, gets the appropriate property resolver
        /// for the specified inspector property based on the locator's specific strategy.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>An IPropertyResolver instance suitable for handling the property.</returns>
        public abstract IPropertyStructureResolver GetResolver(InspectorProperty property);
    }
}

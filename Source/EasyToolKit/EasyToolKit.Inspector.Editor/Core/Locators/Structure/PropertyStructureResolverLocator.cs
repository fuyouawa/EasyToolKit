namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property resolver locators that provides a foundation
    /// for implementing specific resolver location strategies.
    /// </summary>
    public abstract class PropertyStructureResolverLocator : IPropertyStructureResolverLocator
    {
        public virtual bool CanResolver(InspectorProperty property) => true;

        /// <summary>
        /// When implemented in a derived class, gets the appropriate property resolver
        /// for the specified inspector property based on the locator's specific strategy.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>An IPropertyResolver instance suitable for handling the property.</returns>
        public abstract IPropertyStructureResolver GetResolver(InspectorProperty property);
    }
}

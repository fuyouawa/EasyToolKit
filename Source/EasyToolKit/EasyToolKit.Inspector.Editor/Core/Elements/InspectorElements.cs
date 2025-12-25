namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides centralized access to the core factory services of the EasyToolKit inspector system.
    /// This static class serves as the main entry point for accessing element configuration and tree creation services.
    /// </summary>
    public static class InspectorElements
    {
        /// <summary>
        /// Gets the element configurator factory for creating element Definitions.
        /// Provides configuration instances that ultimately create Definitions (through CreateDefinition methods)
        /// for all supported element types including values, properties, groups, methods, and collection items.
        /// The configurations serve as builders that define the behavior and properties of inspector elements.
        /// </summary>
        public static IElementConfigurator Configurator { get; }

        /// <summary>
        /// Gets the element tree factory for creating element trees.
        /// Provides methods to create complete inspector element hierarchies for single or multiple target objects.
        /// The tree factory handles the creation of root elements and manages the overall structure of inspector UI.
        /// </summary>
        public static IElementTreeFactory TreeFactory { get; }

        static InspectorElements()
        {
            Configurator = new Implementations.ElementConfigurator();
            TreeFactory = new Implementations.ElementTreeFactory();
        }
    }
}

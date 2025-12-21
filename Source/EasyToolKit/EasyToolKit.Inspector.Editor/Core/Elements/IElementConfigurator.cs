namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Factory interface for creating element configuration instances.
    /// Provides methods to create configurations for all supported element types in the inspector system.
    /// </summary>
    public interface IElementConfigurator
    {
        /// <summary>
        /// Creates a new <see cref="IValueConfiguration"/> instance for value elements.
        /// Value elements represent data-containing elements like fields, properties, or parameters.
        /// </summary>
        /// <returns>A new value configuration instance.</returns>
        IValueConfiguration Value();

        /// <summary>
        /// Creates a new <see cref="IRootConfiguration"/> instance for root elements.
        /// Root elements represent the top-level inspector entries for Unity instances.
        /// </summary>
        /// <returns>A new root configuration instance.</returns>
        IRootConfiguration Root();

        /// <summary>
        /// Creates a new <see cref="IGroupConfiguration"/> instance for group elements.
        /// Group elements organize related inspector elements using begin/end attribute pairs.
        /// </summary>
        /// <returns>A new group configuration instance.</returns>
        IGroupConfiguration Group();

        /// <summary>
        /// Creates a new <see cref="IPropertyConfiguration"/> instance for property elements.
        /// Property elements unify fields and properties, providing consistent access to member data.
        /// </summary>
        /// <returns>A new property configuration instance.</returns>
        IPropertyConfiguration Property();

        /// <summary>
        /// Creates a new <see cref="IMethodConfiguration"/> instance for method elements.
        /// Method elements represent functions that can be invoked or displayed in the inspector.
        /// </summary>
        /// <returns>A new method configuration instance.</returns>
        IMethodConfiguration Method();

        /// <summary>
        /// Creates a new <see cref="ICollectionItemConfiguration"/> instance for collection item elements.
        /// Collection item elements represent individual elements within collections (lists, arrays).
        /// </summary>
        /// <returns>A new collection item configuration instance.</returns>
        ICollectionItemConfiguration CollectionItem();
    }
}

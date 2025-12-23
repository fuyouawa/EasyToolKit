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
        /// Value elements represent fields, properties, or dynamically created custom values.
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
        /// Creates a new <see cref="IFieldConfiguration"/> instance for field elements.
        /// Field elements provide consistent access to field member data.
        /// </summary>
        /// <returns>A new field configuration instance.</returns>
        IFieldConfiguration Field();

        /// <summary>
        /// Creates a new <see cref="IPropertyConfiguration"/> instance for property elements.
        /// Property elements provide consistent access to property member data.
        /// </summary>
        /// <returns>A new property configuration instance.</returns>
        IPropertyConfiguration Property();

        /// <summary>
        /// Creates a new <see cref="IFieldCollectionConfiguration"/> instance for field collection elements.
        /// Field collection elements represent collection fields on an object.
        /// </summary>
        /// <returns>A new field collection configuration instance.</returns>
        IFieldCollectionConfiguration FieldCollection();

        /// <summary>
        /// Creates a new <see cref="IPropertyCollectionConfiguration"/> instance for property collection elements.
        /// Property collection elements represent collection properties on an object.
        /// </summary>
        /// <returns>A new property collection configuration instance.</returns>
        IPropertyCollectionConfiguration PropertyCollection();

        /// <summary>
        /// Creates a new <see cref="ICollectionConfiguration"/> instance for collection elements.
        /// Collection elements represent dynamically created custom containers that hold multiple items.
        /// </summary>
        /// <returns>A new collection configuration instance.</returns>
        ICollectionConfiguration Collection();

        /// <summary>
        /// Creates a new <see cref="IMethodConfiguration"/> instance for method elements.
        /// Method elements represent functions that can be invoked or displayed in the inspector.
        /// </summary>
        /// <returns>A new method configuration instance.</returns>
        IMethodConfiguration Method();

        /// <summary>
        /// Creates a new <see cref="ICollectionItemConfiguration"/> instance for collection item elements.
        /// Collection item elements represent individual elements within collections (e.g., lists, arrays).
        /// </summary>
        /// <returns>A new collection item configuration instance.</returns>
        ICollectionItemConfiguration CollectionItem();

        /// <summary>
        /// Creates a new <see cref="IMethodParameterConfiguration"/> instance for method parameter elements.
        /// Method parameter elements represent individual parameter values for method invocation.
        /// </summary>
        /// <returns>A new method parameter configuration instance.</returns>
        IMethodParameterConfiguration MethodParameter();
    }
}

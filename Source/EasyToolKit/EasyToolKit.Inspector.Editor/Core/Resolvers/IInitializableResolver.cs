namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for resolvers that can be initialized and deinitialized with an associated <see cref="InspectorProperty"/>
    /// </summary>
    /// <remarks>
    /// This interface combines the initialization capabilities of <see cref="IInitializable"/> with property association,
    /// providing a standard pattern for resolvers in the <see cref="InspectorProperty"/> system.
    /// </remarks>
    public interface IInitializableResolver : IInitializable
    {
        /// <summary>
        /// Gets or sets the <see cref="InspectorProperty"/> associated with this resolver
        /// </summary>
        /// <remarks>
        /// This property is typically set during the resolver's initialization phase
        /// and provides access to the property being resolved.
        /// </remarks>
        InspectorProperty Property { get; set; }
    }
}

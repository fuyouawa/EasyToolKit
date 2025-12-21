namespace EasyToolKit.Core
{
    /// <summary>
    /// Specifies the lifetime of a service in the container.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created for each resolution.
        /// </summary>
        Transient,

        /// <summary>
        /// A single instance is created and reused for all resolutions.
        /// </summary>
        Singleton,

        /// <summary>
        /// A single instance is created per scope.
        /// </summary>
        Scoped
    }
}

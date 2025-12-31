namespace EasyToolKit.Core
{
    /// <summary>
    /// Base interface for all serialization configuration objects.
    /// Provides common identification and metadata for configuration instances.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets the unique identifier for this configuration instance.
        /// </summary>
        string ConfigurationId { get; }

        /// <summary>
        /// Gets the configuration type name for categorization and registration.
        /// </summary>
        string ConfigurationType { get; }

        /// <summary>
        /// Gets a value indicating whether this configuration has been validated.
        /// </summary>
        bool IsValidated { get; }
    }
}

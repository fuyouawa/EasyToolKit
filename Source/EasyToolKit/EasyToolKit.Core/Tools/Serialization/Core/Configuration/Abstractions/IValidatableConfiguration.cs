namespace EasyToolKit.Core
{
    /// <summary>
    /// Interface for configurations that support validation.
    /// Provides mechanisms to validate configuration state and retrieve validation results.
    /// </summary>
    public interface IValidatableConfiguration : IConfiguration
    {
        /// <summary>
        /// Validates the current configuration state.
        /// </summary>
        /// <returns>Validation result containing success status and error messages.</returns>
        ConfigurationValidationResult Validate();

        /// <summary>
        /// Validates the configuration and throws an exception if validation fails.
        /// </summary>
        /// <exception cref="ConfigurationException">Thrown when validation fails.</exception>
        void ValidateOrThrow();
    }
}

namespace EasyToolKit.Core
{
    /// <summary>
    /// Interface for validating configuration objects.
    /// Implementations provide specific validation logic for configuration types.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration to validate.</typeparam>
    public interface IConfigurationValidator<in TConfiguration>
        where TConfiguration : IConfiguration
    {
        /// <summary>
        /// Validates the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration to validate.</param>
        /// <returns>Validation result containing success status and error/warning messages.</returns>
        ConfigurationValidationResult Validate(TConfiguration configuration);

        /// <summary>
        /// Gets the validation priority. Higher priority validators execute first.
        /// </summary>
        int Priority { get; }
    }
}

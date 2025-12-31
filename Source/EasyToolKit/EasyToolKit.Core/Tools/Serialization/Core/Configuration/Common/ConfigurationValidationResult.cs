using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents the result of a configuration validation operation.
    /// </summary>
    public sealed class ConfigurationValidationResult
    {
        private static readonly ConfigurationValidationResult ValidResult =
            new ConfigurationValidationResult(true, new ConfigurationValidationError[0], new ConfigurationValidationWarning[0]);

        /// <summary>
        /// Gets a value indicating whether the validation passed.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IReadOnlyList<ConfigurationValidationError> Errors { get; }

        /// <summary>
        /// Gets the collection of validation warnings.
        /// </summary>
        public IReadOnlyList<ConfigurationValidationWarning> Warnings { get; }

        private ConfigurationValidationResult(
            bool isValid,
            ConfigurationValidationError[] errors,
            ConfigurationValidationWarning[] warnings)
        {
            IsValid = isValid;
            Errors = errors;
            Warnings = warnings;
        }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        public static ConfigurationValidationResult Valid() => ValidResult;

        /// <summary>
        /// Creates a failed validation result with the specified errors.
        /// </summary>
        public static ConfigurationValidationResult Invalid(params ConfigurationValidationError[] errors)
        {
            return new ConfigurationValidationResult(false, errors, new ConfigurationValidationWarning[0]);
        }

        /// <summary>
        /// Creates a failed validation result with the specified errors and warnings.
        /// </summary>
        public static ConfigurationValidationResult Invalid(
            ConfigurationValidationError[] errors,
            ConfigurationValidationWarning[] warnings)
        {
            return new ConfigurationValidationResult(false, errors, warnings);
        }
    }
}

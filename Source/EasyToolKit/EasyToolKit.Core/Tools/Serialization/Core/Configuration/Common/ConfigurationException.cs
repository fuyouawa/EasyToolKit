using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Exception thrown when configuration validation fails or configuration is invalid.
    /// </summary>
    public sealed class ConfigurationException : Exception
    {
        /// <summary>
        /// Gets the configuration type that failed validation.
        /// </summary>
        public string ConfigurationType { get; }

        /// <summary>
        /// Gets the collection of validation errors that caused the exception.
        /// </summary>
        public IReadOnlyList<ConfigurationValidationError> ValidationErrors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message.
        /// </summary>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message and inner exception.
        /// </summary>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with configuration type and validation errors.
        /// </summary>
        public ConfigurationException(
            string configurationType,
            IReadOnlyList<ConfigurationValidationError> validationErrors)
            : base($"Configuration validation failed for {configurationType}")
        {
            ConfigurationType = configurationType;
            ValidationErrors = validationErrors;
        }
    }
}

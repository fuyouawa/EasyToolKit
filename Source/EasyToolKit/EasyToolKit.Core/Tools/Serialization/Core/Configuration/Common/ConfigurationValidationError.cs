namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a validation error in a configuration.
    /// </summary>
    public sealed class ConfigurationValidationError
    {
        /// <summary>
        /// Gets the name of the property or field that caused the error.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the error message describing the validation failure.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the error code for categorizing the error type.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationError"/> class.
        /// </summary>
        public ConfigurationValidationError(string propertyName, string errorMessage, string errorCode = null)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode ?? string.Empty;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(ErrorCode)
                ? $"{PropertyName}: {ErrorMessage}"
                : $"{PropertyName} ({ErrorCode}): {ErrorMessage}";
        }
    }
}

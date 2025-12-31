namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a validation warning in a configuration.
    /// </summary>
    public sealed class ConfigurationValidationWarning
    {
        /// <summary>
        /// Gets the name of the property or field that caused the warning.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the warning message describing the validation warning.
        /// </summary>
        public string WarningMessage { get; }

        /// <summary>
        /// Gets the warning code for categorizing the warning type.
        /// </summary>
        public string WarningCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationWarning"/> class.
        /// </summary>
        public ConfigurationValidationWarning(string propertyName, string warningMessage, string warningCode = null)
        {
            PropertyName = propertyName;
            WarningMessage = warningMessage;
            WarningCode = warningCode ?? string.Empty;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(WarningCode)
                ? $"{PropertyName}: {WarningMessage}"
                : $"{PropertyName} ({WarningCode}): {WarningMessage}";
        }
    }
}

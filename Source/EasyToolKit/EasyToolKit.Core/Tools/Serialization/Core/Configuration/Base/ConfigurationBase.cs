using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Abstract base class for all configuration objects.
    /// Provides common implementation for identification and metadata.
    /// </summary>
    public abstract class ConfigurationBase : IConfiguration
    {
        private string _configurationId;
        private bool _isValidated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBase"/> class.
        /// </summary>
        protected ConfigurationBase()
        {
            _configurationId = Guid.NewGuid().ToString("N");
            _isValidated = false;
        }

        /// <inheritdoc/>
        public string ConfigurationId
        {
            get => _configurationId;
            protected set => _configurationId = value;
        }

        /// <inheritdoc/>
        public string ConfigurationType => GetType().Name;

        /// <inheritdoc/>
        public bool IsValidated => _isValidated;

        /// <summary>
        /// Marks the configuration as validated.
        /// </summary>
        protected void SetValidated()
        {
            _isValidated = true;
        }

        /// <summary>
        /// Resets the validation status, requiring re-validation.
        /// </summary>
        protected void ResetValidation()
        {
            _isValidated = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Abstract base class for configurations that support validation.
    /// Provides default validation implementation and validator management.
    /// </summary>
    public abstract class ValidatableConfigurationBase : ConfigurationBase, IValidatableConfiguration
    {
        private readonly List<IConfigurationValidator<IConfiguration>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableConfigurationBase"/> class.
        /// </summary>
        protected ValidatableConfigurationBase()
        {
            _validators = new List<IConfigurationValidator<IConfiguration>>();
        }

        /// <summary>
        /// Adds a validator to this configuration instance.
        /// </summary>
        protected void AddValidator(IConfigurationValidator<IConfiguration> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _validators.Add(validator);
        }

        /// <inheritdoc/>
        public virtual ConfigurationValidationResult Validate()
        {
            var errors = new List<ConfigurationValidationError>();
            var warnings = new List<ConfigurationValidationWarning>();

            // Run all validators in priority order
            foreach (var validator in _validators.OrderByDescending(v => v.Priority))
            {
                var result = validator.Validate(this);
                if (!result.IsValid)
                {
                    errors.AddRange(result.Errors);
                }
                warnings.AddRange(result.Warnings);
            }

            // Run custom validation
            var customResult = ValidateCore();
            errors.AddRange(customResult.Errors);
            warnings.AddRange(customResult.Warnings);

            if (errors.Count == 0)
            {
                SetValidated();
                return ConfigurationValidationResult.Valid();
            }

            return ConfigurationValidationResult.Invalid(
                errors.ToArray(),
                warnings.ToArray());
        }

        /// <inheritdoc/>
        public void ValidateOrThrow()
        {
            var result = Validate();
            if (!result.IsValid)
            {
                var errorMessages = string.Join("\n", result.Errors.Select(e => $"  - {e}"));
                throw new ConfigurationException(
                    $"Configuration validation failed for {ConfigurationType}:\n{errorMessages}");
            }
        }

        /// <summary>
        /// Override to provide custom validation logic specific to the configuration type.
        /// Called after all registered validators have been executed.
        /// </summary>
        /// <returns>Validation result with any errors or warnings from custom validation logic.</returns>
        protected virtual ConfigurationValidationResult ValidateCore()
        {
            return ConfigurationValidationResult.Valid();
        }
    }
}

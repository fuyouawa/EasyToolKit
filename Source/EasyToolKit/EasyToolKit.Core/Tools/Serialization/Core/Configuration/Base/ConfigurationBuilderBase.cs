using System;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Abstract base class for configuration builders.
    /// Provides common builder functionality and validation integration.
    /// </summary>
    /// <typeparam name="TConfigurationInterface">The interface type of the configuration.</typeparam>
    /// <typeparam name="TConfigurationImpl">The concrete implementation type of the configuration.</typeparam>
    public abstract class ConfigurationBuilderBase<TConfigurationInterface, TConfigurationImpl>
        where TConfigurationInterface : IConfiguration
        where TConfigurationImpl : class, TConfigurationInterface
    {
        private readonly List<IConfigurationValidator<TConfigurationInterface>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBuilderBase{TConfigurationInterface, TConfigurationImpl}"/> class.
        /// </summary>
        protected ConfigurationBuilderBase()
        {
            _validators = new List<IConfigurationValidator<TConfigurationInterface>>();
        }

        /// <summary>
        /// Adds a custom validator to the builder that will be used during BuildAndValidate.
        /// </summary>
        public ConfigurationBuilderBase<TConfigurationInterface, TConfigurationImpl> WithValidator(
            IConfigurationValidator<TConfigurationInterface> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _validators.Add(validator);
            return this;
        }

        /// <summary>
        /// Builds the configuration instance without validation.
        /// </summary>
        public abstract TConfigurationInterface Build();

        /// <summary>
        /// Builds and validates the configuration instance.
        /// </summary>
        public virtual TConfigurationInterface BuildAndValidate()
        {
            var config = Build();

            if (config is IValidatableConfiguration validatableConfig)
            {
                // Add builder validators if configuration supports it
                // (Note: Configuration would need to expose AddValidator method for this to work)
                validatableConfig.ValidateOrThrow();
            }

            return config;
        }

        /// <summary>
        /// Resets the builder to its initial state.
        /// </summary>
        public abstract IConfigurationBuilder<TConfigurationInterface> Reset();
    }
}

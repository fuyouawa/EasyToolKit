namespace EasyToolKit.Core
{
    /// <summary>
    /// Interface for building configuration objects using the builder pattern.
    /// Provides fluent API for constructing and validating configurations.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration this builder creates.</typeparam>
    public interface IConfigurationBuilder<out TConfiguration>
        where TConfiguration : IConfiguration
    {
        /// <summary>
        /// Builds the configuration instance.
        /// </summary>
        /// <returns>The constructed configuration instance.</returns>
        TConfiguration Build();

        /// <summary>
        /// Builds and validates the configuration instance.
        /// </summary>
        /// <returns>The constructed and validated configuration instance.</returns>
        TConfiguration BuildAndValidate();

        /// <summary>
        /// Resets the builder to its initial state.
        /// </summary>
        IConfigurationBuilder<TConfiguration> Reset();
    }
}

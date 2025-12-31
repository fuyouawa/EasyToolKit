using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Settings container for EasySerializer system.
    /// Provides configuration for member filtering and serialization behavior.
    /// </summary>
    public sealed class EasySerializeSettings
    {
        private readonly IMemberFilterConfiguration _memberFilterConfiguration;
        private readonly ISerializedMemberInfoAccessor _serializedMemberInfoAccessor;

        /// <summary>
        /// Gets the member filter configuration.
        /// </summary>
        public IMemberFilterConfiguration MemberFilterConfiguration => _memberFilterConfiguration;

        /// <summary>
        /// Gets the member filter delegate for compatibility with existing code.
        /// </summary>
        public MemberFilter MemberFilter => _memberFilterConfiguration.CreateFilter();

        /// <summary>
        /// Gets the serialized member info accessor (internal use).
        /// </summary>
        internal ISerializedMemberInfoAccessor SerializedMemberInfoAccessor => _serializedMemberInfoAccessor;

        /// <summary>
        /// Initializes a new instance with default configuration.
        /// </summary>
        public EasySerializeSettings()
            : this(CreateDefaultConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified member filter configuration.
        /// </summary>
        public EasySerializeSettings(IMemberFilterConfiguration memberFilterConfiguration)
        {
            _memberFilterConfiguration = memberFilterConfiguration ?? throw new ArgumentNullException(nameof(memberFilterConfiguration));
            _memberFilterConfiguration.ValidateOrThrow();
            _serializedMemberInfoAccessor = new SerializedMemberInfoAccessor(MemberFilter);
        }

        /// <summary>
        /// Initializes a new instance with a preset configuration by name.
        /// </summary>
        public EasySerializeSettings(string presetName)
        {
            if (!MemberFilterPresetRegistry.Instance.TryGetPreset(presetName, out var config))
            {
                throw new ArgumentException($"Preset '{presetName}' not found.", nameof(presetName));
            }

            _memberFilterConfiguration = config;
            _serializedMemberInfoAccessor = new SerializedMemberInfoAccessor(MemberFilter);
        }

        /// <summary>
        /// Creates settings from a configuration builder.
        /// </summary>
        public static EasySerializeSettings FromBuilder(MemberFilterConfigurationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var config = builder.BuildAndValidate();
            return new EasySerializeSettings(config);
        }

        private static IMemberFilterConfiguration CreateDefaultConfiguration()
        {
            return MemberFilterPresetRegistry.Instance.GetPreset("Default");
        }
    }
}

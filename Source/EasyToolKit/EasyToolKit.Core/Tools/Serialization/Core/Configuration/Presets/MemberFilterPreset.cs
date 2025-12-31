namespace EasyToolKit.Core
{
    /// <summary>
    /// Represents a named preset configuration for member filtering.
    /// </summary>
    public sealed class MemberFilterPreset
    {
        /// <summary>
        /// Gets the name of the preset.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the member filter configuration for this preset.
        /// </summary>
        public IMemberFilterConfiguration Configuration { get; }

        /// <summary>
        /// Gets the description of the preset.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFilterPreset"/> class.
        /// </summary>
        public MemberFilterPreset(string name, IMemberFilterConfiguration configuration, string description = null)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
            Description = description ?? string.Empty;
        }
    }
}

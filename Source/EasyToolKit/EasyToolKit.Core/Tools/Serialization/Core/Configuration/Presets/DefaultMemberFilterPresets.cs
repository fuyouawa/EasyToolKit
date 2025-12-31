using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides default member filter preset configurations.
    /// Static factory for creating standard presets used throughout the framework.
    /// </summary>
    public static class DefaultMemberFilterPresets
    {
        /// <summary>
        /// Creates the default member filter preset.
        /// Includes public fields and fields with [SerializeField] attribute.
        /// </summary>
        public static MemberFilterPreset CreateDefault()
        {
            var config = new MemberFilterConfigurationBuilder()
                .IncludeFields()
                .IncludePublic()
                .IncludeSerializedFields()
                .BuildAndValidate();

            return new MemberFilterPreset(
                "Default",
                config,
                "Public fields and [SerializeField] members");
        }

        /// <summary>
        /// Creates a preset that includes all gettable members (public and non-public).
        /// </summary>
        public static MemberFilterPreset CreateAllGettable()
        {
            var config = new MemberFilterConfigurationBuilder()
                .IncludeFields()
                .IncludeAllProperties()
                .IncludePublic()
                .IncludeNonPublic()
                .BuildAndValidate();

            return new MemberFilterPreset(
                "AllGettable",
                config,
                "All gettable fields and properties (public and non-public)");
        }

        /// <summary>
        /// Creates a preset that includes all public gettable members.
        /// </summary>
        public static MemberFilterPreset CreateAllPublicGettable()
        {
            var config = new MemberFilterConfigurationBuilder()
                .IncludeFields()
                .IncludeAllProperties()
                .IncludePublic()
                .BuildAndValidate();

            return new MemberFilterPreset(
                "AllPublicGettable",
                config,
                "All public gettable fields and properties");
        }

        /// <summary>
        /// Creates all default presets.
        /// </summary>
        public static IReadOnlyList<MemberFilterPreset> CreateAll()
        {
            return new[]
            {
                CreateDefault(),
                CreateAllGettable(),
                CreateAllPublicGettable(),
            };
        }
    }
}

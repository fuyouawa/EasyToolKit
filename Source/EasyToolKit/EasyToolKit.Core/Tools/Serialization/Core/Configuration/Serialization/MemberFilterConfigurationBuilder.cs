using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Builder for creating <see cref="IMemberFilterConfiguration"/> instances.
    /// Provides fluent API for configuring member filtering rules.
    /// </summary>
    public sealed class MemberFilterConfigurationBuilder :
        ConfigurationBuilderBase<IMemberFilterConfiguration, MemberFilterConfiguration>,
        IConfigurationBuilder<IMemberFilterConfiguration>
    {
        private readonly MemberFilterConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFilterConfigurationBuilder"/> class.
        /// </summary>
        public MemberFilterConfigurationBuilder()
        {
            _configuration = new MemberFilterConfiguration();
        }

        /// <summary>
        /// Configures the filter to include public members.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludePublic()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.Public);
            return this;
        }

        /// <summary>
        /// Configures the filter to include non-public members.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeNonPublic()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.NonPublic);
            return this;
        }

        /// <summary>
        /// Configures the filter to include fields.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeFields()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.Field);
            return this;
        }

        /// <summary>
        /// Configures the filter to include read-only properties.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeReadOnlyProperties()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.ReadOnlyProperty);
            return this;
        }

        /// <summary>
        /// Configures the filter to include write-only properties.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeWriteOnlyProperties()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.WriteOnlyProperty);
            return this;
        }

        /// <summary>
        /// Configures the filter to include read-write properties.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeReadWriteProperties()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.ReadWriteProperty);
            return this;
        }

        /// <summary>
        /// Configures the filter to include all properties.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeAllProperties()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.AllProperty);
            return this;
        }

        /// <summary>
        /// Configures the filter to include members with [SerializeField] attribute.
        /// </summary>
        public MemberFilterConfigurationBuilder IncludeSerializedFields()
        {
            _configuration.SetFilterFlags(_configuration.FilterFlags | MemberFilterFlags.SerializeField);
            return this;
        }

        /// <summary>
        /// Adds a type to the exclusion list.
        /// </summary>
        public MemberFilterConfigurationBuilder ExcludeType(Type type)
        {
            _configuration.AddExcludedType(type);
            return this;
        }

        /// <summary>
        /// Adds multiple types to the exclusion list.
        /// </summary>
        public MemberFilterConfigurationBuilder ExcludeTypes(params Type[] types)
        {
            _configuration.AddExcludedTypes(types);
            return this;
        }

        /// <summary>
        /// Sets the filter flags directly.
        /// </summary>
        public MemberFilterConfigurationBuilder WithFlags(MemberFilterFlags flags)
        {
            _configuration.SetFilterFlags(flags);
            return this;
        }

        /// <inheritdoc/>
        public override IMemberFilterConfiguration Build()
        {
            return _configuration;
        }

        /// <inheritdoc/>
        public override IConfigurationBuilder<IMemberFilterConfiguration> Reset()
        {
            return new MemberFilterConfigurationBuilder();
        }
    }
}

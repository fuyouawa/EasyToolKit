using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Default implementation of preset registry for member filter configurations.
    /// Thread-safe singleton instance for global preset management.
    /// </summary>
    public sealed class MemberFilterPresetRegistry : IMemberFilterPresetRegistry
    {
        private static readonly Lazy<MemberFilterPresetRegistry> InstanceHolder =
            new Lazy<MemberFilterPresetRegistry>(() => new MemberFilterPresetRegistry());

        private readonly Dictionary<string, IMemberFilterConfiguration> _presets;
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of the preset registry.
        /// </summary>
        public static IMemberFilterPresetRegistry Instance => InstanceHolder.Value;

        private MemberFilterPresetRegistry()
        {
            _presets = new Dictionary<string, IMemberFilterConfiguration>(StringComparer.Ordinal);
            RegisterDefaultPresets();
        }

        /// <inheritdoc/>
        public void RegisterPreset(string name, IMemberFilterConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Preset name cannot be null or whitespace.", nameof(name));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            lock (_lock)
            {
                _presets[name] = configuration;
            }
        }

        /// <inheritdoc/>
        public bool TryGetPreset(string name, out IMemberFilterConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                configuration = null;
                return false;
            }

            lock (_lock)
            {
                return _presets.TryGetValue(name, out configuration);
            }
        }

        /// <inheritdoc/>
        public IMemberFilterConfiguration GetPreset(string name)
        {
            if (TryGetPreset(name, out var configuration))
            {
                return configuration;
            }

            throw new KeyNotFoundException($"Member filter preset '{name}' is not registered.");
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> GetPresetNames()
        {
            lock (_lock)
            {
                return _presets.Keys.ToList().AsReadOnly();
            }
        }

        /// <inheritdoc/>
        public bool HasPreset(string name)
        {
            return TryGetPreset(name, out _);
        }

        /// <inheritdoc/>
        public bool UnregisterPreset(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            lock (_lock)
            {
                return _presets.Remove(name);
            }
        }

        private void RegisterDefaultPresets()
        {
            var defaultPresets = DefaultMemberFilterPresets.CreateAll();
            foreach (var preset in defaultPresets)
            {
                _presets[preset.Name] = preset.Configuration;
            }
        }
    }
}

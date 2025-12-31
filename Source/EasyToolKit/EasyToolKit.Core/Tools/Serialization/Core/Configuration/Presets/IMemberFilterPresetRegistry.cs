using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Registry for managing member filter configuration presets.
    /// Allows registration and retrieval of named preset configurations.
    /// </summary>
    public interface IMemberFilterPresetRegistry
    {
        /// <summary>
        /// Registers a preset configuration with the specified name.
        /// </summary>
        void RegisterPreset(string name, IMemberFilterConfiguration configuration);

        /// <summary>
        /// Tries to get a preset configuration by name.
        /// </summary>
        bool TryGetPreset(string name, out IMemberFilterConfiguration configuration);

        /// <summary>
        /// Gets a preset configuration by name, throwing if not found.
        /// </summary>
        IMemberFilterConfiguration GetPreset(string name);

        /// <summary>
        /// Gets all registered preset names.
        /// </summary>
        IReadOnlyCollection<string> GetPresetNames();

        /// <summary>
        /// Checks if a preset with the specified name exists.
        /// </summary>
        bool HasPreset(string name);

        /// <summary>
        /// Unregisters a preset by name.
        /// </summary>
        bool UnregisterPreset(string name);
    }
}

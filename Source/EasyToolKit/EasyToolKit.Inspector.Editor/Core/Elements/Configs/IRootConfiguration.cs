using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating root element definitions.
    /// Root elements represent the top-level inspector entries for Unity instances.
    /// </summary>
    public interface IRootConfiguration : IValueConfiguration
    {
        /// <summary>
        /// Creates a new <see cref="IRootDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new root definition instance.</returns>
        new IRootDefinition CreateDefinition();
    }
}
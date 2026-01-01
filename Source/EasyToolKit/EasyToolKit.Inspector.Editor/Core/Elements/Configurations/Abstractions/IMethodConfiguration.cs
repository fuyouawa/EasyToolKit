using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating method element definitions.
    /// Methods represent functions that can be invoked or displayed in the inspector interface.
    /// </summary>
    public interface IMethodConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the <see cref="MethodInfo"/> that describes the method to be created.
        /// This contains metadata about the method including parameters, return type, and attributes.
        /// </summary>
        MethodInfo MethodInfo { get; set; }

        /// <summary>
        /// Creates a new <see cref="IMethodDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new method definition instance.</returns>
        IMethodDefinition CreateDefinition();
    }
}
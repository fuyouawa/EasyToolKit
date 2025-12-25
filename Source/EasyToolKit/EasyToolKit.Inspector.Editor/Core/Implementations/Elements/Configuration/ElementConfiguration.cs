namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base configuration interface for all element configurations in the inspector system.
    /// Provides common properties required for creating element definitions.
    /// </summary>
    public abstract class ElementConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the display name of the element.
        /// This name is shown in the inspector interface and used for identification.
        /// </summary>
        public string Name { get; set; }
    }
}

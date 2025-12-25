namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents the definition of an element in the inspector hierarchy.
    /// It serves as the base interface for all element definitions.
    /// </summary>
    public abstract class ElementDefinition : IElementDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        protected ElementDefinition(ElementFlags flags, string name)
        {
            Flags = flags;
            Name = name;
        }

        /// <summary>
        /// Gets the flags of the element.
        /// </summary>
        public ElementFlags Flags { get; }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string Name { get; }
    }
}

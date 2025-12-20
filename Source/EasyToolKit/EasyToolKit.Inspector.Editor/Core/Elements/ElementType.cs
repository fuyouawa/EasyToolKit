namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the type of an element in the inspector tree.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// The root element of the inspector tree.
        /// </summary>
        Root,

        /// <summary>
        /// A group element that contains other elements.
        /// </summary>
        Group,

        /// <summary>
        /// A property element.
        /// </summary>
        Property,

        /// <summary>
        /// A method element.
        /// </summary>
        Method,

        /// <summary>
        /// A value element (field, property, or method).
        /// </summary>
        Value,

        /// <summary>
        /// A collection item element.
        /// </summary>
        CollectionItem
    }
}

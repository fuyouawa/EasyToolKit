namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the source of an attribute in the inspector property system
    /// </summary>
    public enum ElementAttributeSource
    {
        /// <summary>
        /// Attribute is defined on a member (field, property, method)
        /// </summary>
        Member,

        /// <summary>
        /// Attribute is defined on a type (class, struct, interface)
        /// </summary>
        Type,

        /// <summary>
        /// Attribute is defined on a list/collection type/field but is passed to each element
        /// </summary>
        ListPassToElement,

        Custom
    }
}

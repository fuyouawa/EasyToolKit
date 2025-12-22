using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the flags that define the nature and role of an element in the inspector tree.
    /// Elements can have multiple flags to represent complex characteristics (e.g., a Property with Value).
    /// </summary>
    [Flags]
    public enum ElementFlags
    {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Root element of the inspector tree.
        /// </summary>
        /// <remarks>
        /// Always combined with Value flag.
        /// </remarks>
        Root = 1 << 0,

        /// <summary>
        /// Data-containing element that can be displayed and edited.
        /// </summary>
        /// <remarks>
        /// Always combined with at least one data-specific flag (Root, Field, Property, Collection, or CollectionItem).
        /// </remarks>
        Value = 1 << 1,

        /// <summary>
        /// Field element representing a class field.
        /// </summary>
        /// <remarks>
        /// Always combined with Value flag.
        /// </remarks>
        Field = 1 << 2,

        /// <summary>
        /// Property element representing a class property.
        /// </summary>
        /// <remarks>
        /// Always combined with Value flag.
        /// </remarks>
        Property = 1 << 3,

        /// <summary>
        /// Collection element (Array, List, Dictionary).
        /// </summary>
        /// <remarks>
        /// Always combined with Value flag.
        /// </remarks>
        Collection = 1 << 4,

        /// <summary>
        /// Individual item within a collection.
        /// </summary>
        /// <remarks>
        /// Always combined with Value flag.
        /// </remarks>
        CollectionItem = 1 << 5,

        /// <summary>
        /// Method element representing a class method that can be invoked in the inspector.
        /// </summary>
        Method = 1 << 6,

        /// <summary>
        /// Group element for organizing related elements in the inspector UI.
        /// </summary>
        Group = 1 << 7,
    }
}

using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the roles that define the nature and role of an element in the inspector tree.
    /// Elements can have multiple roles to represent complex characteristics (e.g., a Property with Value).
    /// </summary>
    [Flags]
    public enum ElementRoles
    {
        /// <summary>
        /// No roles set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Root element of the inspector tree.
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        Root = 1 << 0,

        /// <summary>
        /// Data-containing element that can be displayed and edited.
        /// </summary>
        Value = 1 << 1,

        /// <summary>
        /// Field element representing a class field.
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        Field = 1 << 2,

        /// <summary>
        /// Property element representing a class property.
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        Property = 1 << 3,

        /// <summary>
        /// Collection element (Array, List, Dictionary).
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        Collection = 1 << 4,

        /// <summary>
        /// Individual item within a collection.
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        CollectionItem = 1 << 5,

        /// <summary>
        /// Method element representing a class method that can be invoked in the inspector.
        /// </summary>
        Method = 1 << 6,

        /// <summary>
        /// Method parameter element representing an individual parameter for method invocation.
        /// </summary>
        /// <remarks>
        /// Always combined with <see cref="Value"/>.
        /// </remarks>
        MethodParameter = 1 << 8,

        /// <summary>
        /// Group element for organizing related elements in the inspector UI.
        /// </summary>
        Group = 1 << 7,
    }
}

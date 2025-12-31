namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines priority levels for serializer registration.
    /// Higher values indicate higher priority (earlier evaluation during serializer selection).
    /// </summary>
    public enum SerializerPriorityLevel
    {
        /// <summary>
        /// Generic serializer with lowest priority (fallback option).
        /// </summary>
        Generic = -5000,

        /// <summary>
        /// System basic types serializer (arrays, enums, Type, etc.).
        /// </summary>
        SystemBasic = -4000,

        /// <summary>
        /// Unity object serializer.
        /// </summary>
        UnityObject = -3000,

        /// <summary>
        /// Unity basic type serializer (Vector, Color, etc.).
        /// </summary>
        UnityBasic = -2000,

        /// <summary>
        /// Primitive type serializer (int, float, bool, string, etc.).
        /// </summary>
        Primitive = -1000,

        /// <summary>
        /// Custom serializer with default priority.
        /// </summary>
        Custom = 0,
    }
}

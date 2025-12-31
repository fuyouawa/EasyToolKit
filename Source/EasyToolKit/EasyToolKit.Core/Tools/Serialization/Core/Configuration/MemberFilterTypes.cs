using System;
using System.Reflection;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Flags for determining which members to include during serialization.
    /// </summary>
    [Flags]
    public enum MemberFilterFlags
    {
        None = 0,
        Public = 1 << 0,
        NonPublic = 1 << 1,

        /// <summary>
        /// Members with [SerializeField] attribute, ignoring access modifiers.
        /// </summary>
        SerializeField = 1 << 2,
        Field = 1 << 3,
        ReadOnlyProperty = 1 << 4,
        WriteOnlyProperty = 1 << 5,
        ReadWriteProperty = 1 << 6,
        AllProperty = ReadOnlyProperty | WriteOnlyProperty | ReadWriteProperty
    }

    /// <summary>
    /// Delegate for filtering members during serialization.
    /// </summary>
    public delegate bool MemberFilter(MemberInfo member);
}

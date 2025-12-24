using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base interface for value entry wrappers.
    /// </summary>
    public interface IValueEntryWrapper : IValueEntry
    {
    }

    /// <summary>
    /// Type-safe wrapper for a value entry that exposes a more derived value type.
    /// </summary>
    /// <typeparam name="TValue">The derived value type exposed by this wrapper.</typeparam>
    /// <typeparam name="TBaseValue">The base value type stored in the underlying value entry.</typeparam>
    public interface IValueEntryWrapper<TValue, TBaseValue> : IValueEntry<TValue>, IValueEntryWrapper
        where TBaseValue : notnull
        where TValue : TBaseValue
    {
        /// <summary>
        /// Gets the underlying value entry.
        /// </summary>
        IValueEntry<TBaseValue> BaseValueEntry { get; }
    }
}

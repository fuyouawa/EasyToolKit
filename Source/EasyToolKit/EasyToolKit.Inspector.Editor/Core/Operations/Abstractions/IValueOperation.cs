using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Unified value operation interface to replace IValueAccessor
    /// Provides read and write access to value values
    /// </summary>
    public interface IValueOperation
    {
        /// <summary>
        /// Whether the value is read-only
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Owner type
        /// </summary>
        Type OwnerType { get; }

        /// <summary>
        /// Value type
        /// </summary>
        Type ValueType { get; }

        Type GetValueRuntimeType(ref object owner);

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        object GetWeakValue(ref object owner);

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        void SetWeakValue(ref object owner, object value);
    }

    /// <summary>
    /// Generic value operation interface with type safety
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public interface IValueOperation<TValue> : IValueOperation
    {
        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        TValue GetValue(ref object owner);

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        void SetValue(ref object owner, TValue value);
    }
}

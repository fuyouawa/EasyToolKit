using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Unified property operation interface to replace IValueAccessor
    /// Provides read and write access to property values
    /// </summary>
    public interface IPropertyOperation
    {
        /// <summary>
        /// Whether the property is read-only
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
    /// Generic property operation interface with type safety
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public interface IPropertyOperation<TOwner, TValue> : IPropertyOperation
    {
        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        TValue GetValue(ref TOwner owner);

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        void SetValue(ref TOwner owner, TValue value);
    }
}

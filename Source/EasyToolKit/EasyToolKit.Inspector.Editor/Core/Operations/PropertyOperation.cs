using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property operations
    /// </summary>
    public abstract class PropertyOperation : IPropertyOperation
    {
        /// <summary>
        /// Whether the property is read-only
        /// </summary>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Owner type
        /// </summary>
        public abstract Type OwnerType { get; }

        /// <summary>
        /// Value type
        /// </summary>
        public abstract Type ValueType { get; }

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public abstract object GetWeakValue(ref object owner);

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public abstract void SetWeakValue(ref object owner, object value);
    }

    /// <summary>
    /// Generic abstract base class for property operations with type safety
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class PropertyOperation<TOwner, TValue> : PropertyOperation, IPropertyOperation<TOwner, TValue>
    {
        /// <summary>
        /// Owner type
        /// </summary>
        public override Type OwnerType => typeof(TOwner);

        /// <summary>
        /// Value type
        /// </summary>
        public override Type ValueType => typeof(TValue);

        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public abstract TValue GetValue(ref TOwner owner);

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public abstract void SetValue(ref TOwner owner, TValue value);

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override object GetWeakValue(ref object owner)
        {
            var castOwner = (TOwner)owner;
            return GetValue(ref castOwner);
        }

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetWeakValue(ref object owner, object value)
        {
            var castOwner = (TOwner)owner;
            var castValue = (TValue)value;
            SetValue(ref castOwner, castValue);
            owner = castOwner;
        }
    }
}

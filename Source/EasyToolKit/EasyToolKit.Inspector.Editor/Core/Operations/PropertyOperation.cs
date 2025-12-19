using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property operations
    /// </summary>
    public abstract class PropertyOperation : IPropertyOperation
    {
        protected PropertyOperation(Type ownerType)
        {
            OwnerType = ownerType;
        }

        /// <summary>
        /// Whether the property is read-only
        /// </summary>
        public virtual bool IsReadOnly => false;

        /// <summary>
        /// Owner type
        /// </summary>
        public virtual Type OwnerType { get; }

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
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class PropertyOperation<TValue> : PropertyOperation, IPropertyOperation<TValue>
    {
        protected PropertyOperation(Type ownerType) : base(ownerType)
        {
        }

        /// <summary>
        /// Value type
        /// </summary>
        public override Type ValueType => typeof(TValue);

        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public abstract TValue GetValue(ref object owner);

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public abstract void SetValue(ref object owner, TValue value);

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override object GetWeakValue(ref object owner)
        {
            return GetValue(ref owner);
        }

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetWeakValue(ref object owner, object value)
        {
            var castValue = (TValue)value;
            SetValue(ref owner, castValue);
        }
    }
}

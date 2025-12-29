using System;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for property operations
    /// </summary>
    public abstract class ValueOperationBase : IValueOperation
    {
        protected ValueOperationBase(Type ownerType)
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
        /// Gets the runtime type of the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Runtime type of the value</returns>
        public virtual Type GetValueRuntimeType(ref object owner)
        {
            var value = GetWeakValue(ref owner);
            if (value != null)
            {
                return value.GetType();
            }

            return ValueType;
        }

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

        Type IValueOperation.GetValueRuntimeType(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetValueRuntimeType(ref owner);
        }

        object IValueOperation.GetWeakValue(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetWeakValue(ref owner);
        }

        void IValueOperation.SetWeakValue(ref object owner, object value)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");

            Assert.IsTrue(value == null || value.GetType() == ValueType,
                () => $"Value type mismatch. Expected: {ValueType}, Actual: {value?.GetType()}");
            SetWeakValue(ref owner, value);
        }
    }

    /// <summary>
    /// Generic abstract base class for property operations with type safety
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public abstract class ValueOperationBase<TValue> : ValueOperationBase, IValueOperation<TValue>
    {
        protected ValueOperationBase(Type ownerType) : base(ownerType)
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

        TValue IValueOperation<TValue>.GetValue(ref object owner)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            return GetValue(ref owner);
        }

        void IValueOperation<TValue>.SetValue(ref object owner, TValue value)
        {
            var o = owner;
            Assert.IsTrue(owner.GetType().IsInheritsFrom(OwnerType),
                () => $"Owner type mismatch. Expected: {OwnerType}, Actual: {o.GetType()}");
            SetValue(ref owner, value);
        }
    }
}

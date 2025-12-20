using System;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Generic property operation implementation using delegates
    /// </summary>
    public class GenericPropertyOperation : PropertyOperationBase
    {
        /// <summary>
        /// Value type
        /// </summary>
        public override Type ValueType { get; }

        private readonly WeakValueGetter _getter;
        private readonly WeakValueSetter _setter;
        /// <summary>
        /// Initializes a new instance of GenericPropertyOperation
        /// </summary>
        /// <param name="ownerType">Owner type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="getter">Value getter delegate</param>
        /// <param name="setter">Value setter delegate</param>
        public GenericPropertyOperation(Type ownerType, Type valueType, WeakValueGetter getter, WeakValueSetter setter) : base(ownerType)
        {
            ValueType = valueType;
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Whether the property is read-only
        /// </summary>
        public override bool IsReadOnly => _setter == null;

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override object GetWeakValue(ref object owner)
        {
            return _getter(ref owner);
        }

        /// <summary>
        /// Sets the value
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetWeakValue(ref object owner, object value)
        {
            if (_setter == null)
                throw new NotSupportedException("Property is read-only");

            _setter(ref owner, value);
        }
    }

    /// <summary>
    /// Generic property operation implementation with type safety
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class GenericPropertyOperation<TOwner, TValue> : PropertyOperationBase<TValue>
    {
        private readonly ValueGetter<TOwner, TValue> _getter;
        private readonly ValueSetter<TOwner, TValue> _setter;

        /// <summary>
        /// Initializes a new instance of GenericPropertyOperation
        /// </summary>
        /// <param name="getter">Value getter delegate</param>
        /// <param name="setter">Value setter delegate</param>
        public GenericPropertyOperation(ValueGetter<TOwner, TValue> getter, ValueSetter<TOwner, TValue> setter)
            : base(typeof(TOwner))
        {
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Whether the property is read-only
        /// </summary>
        public override bool IsReadOnly => _setter == null;

        /// <summary>
        /// Gets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <returns>Property value</returns>
        public override TValue GetValue(ref object owner)
        {
            var castedOwner = (TOwner)owner;
            var result = _getter(ref castedOwner);
            return result;
        }

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetValue(ref object owner, TValue value)
        {
            if (_setter == null)
                throw new NotSupportedException("Property is read-only");

            var castedOwner = (TOwner)owner;
            _setter(ref castedOwner, value);
            owner = castedOwner;
        }
    }
}

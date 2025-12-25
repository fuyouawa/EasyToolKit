using System;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Generic property operation implementation with type safety
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    public class GenericValueOperation<TValue> : ValueOperationBase<TValue>
    {
        private readonly WeakValueGetter<TValue> _getter;
        private readonly WeakValueSetter<TValue> _setter;

        /// <summary>
        /// Initializes a new instance of GenericPropertyOperation
        /// </summary>
        /// <param name="getter">Value getter delegate</param>
        /// <param name="setter">Value setter delegate</param>
        public GenericValueOperation(Type ownerType, WeakValueGetter<TValue> getter, WeakValueSetter<TValue> setter)
            : base(ownerType)
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
            return _getter(ref owner);
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

            _setter(ref owner, value);
        }
    }
}

using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property operation implementation for member access (fields and properties)
    /// </summary>
    /// <typeparam name="TOwner">Owner type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    public class MemberPropertyOperation<TOwner, TValue> : PropertyOperation<TOwner, TValue>
    {
        private readonly MemberInfo _memberInfo;
        private readonly ValueGetter<TOwner, TValue> _getter;
        private readonly ValueSetter<TOwner, TValue> _setter;

        /// <summary>
        /// Initializes a new instance of MemberPropertyOperation
        /// </summary>
        /// <param name="memberInfo">Member information</param>
        public MemberPropertyOperation(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    _getter = fieldInfo.GetInstanceValueGetter<TOwner, TValue>();
                    _setter = fieldInfo.GetInstanceValueSetter<TOwner, TValue>();
                    break;
                case PropertyInfo propertyInfo:
                    _getter = propertyInfo.GetInstanceValueGetter<TOwner, TValue>();
                    try
                    {
                        _setter = propertyInfo.GetInstanceValueSetter<TOwner, TValue>();
                    }
                    catch (Exception e)
                    {
                        // Property is read-only
                    }
                    break;
                default:
                    throw new NotSupportedException($"MemberInfo '{memberInfo.Name}' is not supported.");
            }

            _memberInfo = memberInfo;
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
        public override TValue GetValue(ref TOwner owner)
        {
            return _getter(ref owner);
        }

        /// <summary>
        /// Sets the value with type safety
        /// </summary>
        /// <param name="owner">Owner object</param>
        /// <param name="value">Value to set</param>
        public override void SetValue(ref TOwner owner, TValue value)
        {
            if (_setter == null)
            {
                throw new NotSupportedException($"Member '{_memberInfo.Name}' is read-only.");
            }
            _setter(ref owner, value);
        }
    }
}

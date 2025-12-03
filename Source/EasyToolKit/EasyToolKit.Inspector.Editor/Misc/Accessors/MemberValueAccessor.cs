using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    public class MemberValueAccessor<TOwner, TValue> : ValueAccessor<TOwner, TValue>
    {
        private readonly MemberInfo _memberInfo;
        private readonly ValueGetter<TOwner, TValue> _getter;
        private readonly ValueSetter<TOwner, TValue> _setter;

        public MemberValueAccessor(MemberInfo memberInfo)
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
                        // ignored
                    }

                    break;
                default:
                    throw new NotSupportedException($"MemberInfo '{memberInfo.Name}' is not supported.");
            }

            _memberInfo = memberInfo;
        }

        public override void SetValue(ref TOwner target, TValue collection)
        {
            if (_setter == null)
            {
                throw new NotSupportedException($"Member '{_memberInfo.Name}' is not support set value.");
            }
            _setter(ref target, collection);
        }

        public override TValue GetValue(ref TOwner target)
        {
            return _getter(ref target);
        }
    }
}

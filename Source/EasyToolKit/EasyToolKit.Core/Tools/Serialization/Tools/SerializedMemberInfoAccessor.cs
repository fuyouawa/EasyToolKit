using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyToolKit.Core
{
    internal class SerializedMemberInfo
    {
        public Type MemberType;
        public MemberInfo Member;
        public string MemberName;
        public Func<object, object> ValueGetter;
        public Action<object, object> ValueSetter;
        public IEasySerializer Serializer;
    }

    internal interface ISerializedMemberInfoAccessor
    {
        SerializedMemberInfo[] Get(Type targetType);
    }

    internal class SerializedMemberInfoAccessor : ISerializedMemberInfoAccessor
    {
        private readonly MemberFilter _filter;
        private readonly Dictionary<Type, SerializedMemberInfo[]> _memberWithSerializersByClassType = new Dictionary<Type, SerializedMemberInfo[]>();

        private static readonly BindingFlags AllBinding =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public SerializedMemberInfoAccessor(MemberFilter filter)
        {
            _filter = filter;
        }

        public SerializedMemberInfo[] Get(Type targetType)
        {
            if (!_memberWithSerializersByClassType.TryGetValue(targetType, out var array))
            {
                var members = targetType.GetMembers(AllBinding)
                    .Where(memberInfo => _filter(memberInfo))
                    .Select(memberInfo =>
                    {
                        if (memberInfo is FieldInfo fieldInfo)
                        {
                            return new SerializedMemberInfo()
                            {
                                MemberType = fieldInfo.FieldType,
                                Member = fieldInfo,
                                MemberName = fieldInfo.Name,
                                ValueGetter = instance => fieldInfo.GetValue(instance),
                                ValueSetter = (instance, value) => fieldInfo.SetValue(instance, value),
                                Serializer = EasySerializerUtility.GetSerializer(fieldInfo.FieldType)
                            };
                        }

                        if (memberInfo is PropertyInfo propertyInfo)
                        {
                            return new SerializedMemberInfo()
                            {
                                MemberType = propertyInfo.PropertyType,
                                Member = propertyInfo,
                                MemberName = propertyInfo.Name,
                                ValueGetter = instance => propertyInfo.GetValue(instance),
                                ValueSetter = (instance, value) => propertyInfo.SetValue(instance, value),
                                Serializer = EasySerializerUtility.GetSerializer(propertyInfo.PropertyType)
                            };
                        }

                        throw new Exception($"Unsupported member type: {memberInfo.GetType()}");
                    });

                array = members.ToArray();
                _memberWithSerializersByClassType[targetType] = array;
            }

            return array;
        }
    }
}

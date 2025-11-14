using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericPropertyResolver : PropertyResolver
    {
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        protected override void Initialize()
        {
            var targetType = Property.ValueEntry.ValueType;
            var memberInfos = targetType.GetAllMembers(BindingFlagsHelper.AllInstance).Where(Filter).OrderBy(Order);

            var showOdinSerializersInInspector = targetType.IsDefined<ShowOdinSerializedPropertiesInInspector>(true);
            foreach (var memberInfo in memberInfos)
            {
                var showInInspector = memberInfo.IsDefined<ShowInInspectorAttribute>();
                if (memberInfo is FieldInfo fieldInfo)
                {
                    if (fieldInfo.GetCustomAttributes().Any(attr => attr is AuxiliaryAttribute))
                    {
                        _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
                        continue;
                    }

                    if (showOdinSerializersInInspector && fieldInfo.IsDefined<OdinSerializeAttribute>())
                    {
                        _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
                        continue;
                    }

                    if (!InspectorPropertyInfoUtility.IsSerializableField(fieldInfo) && !showInInspector)
                    {
                        continue;
                    }

                    if (!showInInspector)
                    {
                        var memberType = memberInfo.GetMemberType();
                        if (!memberType.IsInheritsFrom<UnityEngine.Object>() &&
                            !memberType.IsValueType &&
                            !memberType.IsDefined<SerializableAttribute>())
                        {
                            continue;
                        }
                    }
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    //TODO support property
                    continue;
                }
                else if (memberInfo is MethodInfo methodInfo)
                {
                    if (!methodInfo.GetCustomAttributes().Any(attr => attr is MethodAttribute))
                        continue;
                }


                _propertyInfos.Add(InspectorPropertyInfo.CreateForMember(memberInfo));
            }
        }

        protected override void Deinitialize()
        {
            _propertyInfos.Clear();
        }

        public override int ChildNameToIndex(string name)
        {
            return _propertyInfos.FindIndex(info => info.PropertyName == name);
        }

        public override int CalculateChildCount()
        {
            return _propertyInfos.Count;
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            return _propertyInfos[childIndex];
        }

        private int Order(MemberInfo memberInfo)
        {
            if (InspectorAttributeUtility.TryGetMemberDefinedAttributePropertyPriority(memberInfo, out var priority))
            {
                return -priority.Priority;
            }
            if (memberInfo is FieldInfo) return -AttributePropertyPriorityLevel.Field;
            if (memberInfo is PropertyInfo) return -AttributePropertyPriorityLevel.Property;
            if (memberInfo is MethodInfo) return -AttributePropertyPriorityLevel.Method;
            return -AttributePropertyPriorityLevel.Default;
        }

        private bool Filter(MemberInfo memberInfo)
        {
            var targetType = Property.ValueEntry.ValueType;
            if (memberInfo.DeclaringType == typeof(object) && targetType != typeof(object)) return false;
            if (!(memberInfo is FieldInfo || memberInfo is PropertyInfo || memberInfo is MethodInfo)) return false;
            if (memberInfo is FieldInfo fieldInfo && fieldInfo.IsSpecialName) return false;
            if (memberInfo is MethodInfo methodInfo && methodInfo.IsSpecialName) return false;
            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.IsSpecialName) return false;
            if (memberInfo.IsDefined<CompilerGeneratedAttribute>()) return false;

            return true;
        }
    }
}

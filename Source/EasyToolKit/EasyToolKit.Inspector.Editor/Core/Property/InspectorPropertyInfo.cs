using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        private MemberInfo _memberInfo;
        private bool? _isArrayElement;

        [CanBeNull] public IPropertyResolverLocator PropertyResolverLocator { get; private set; }
        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }
        [CanBeNull] public Type PropertyType { get; private set; }
        public string PropertyName { get; private set; }
        public bool IsLogicRoot { get; private set; }
        public bool IsUnityProperty { get; private set; }

        private InspectorPropertyInfo()
        {
        }

        public bool IsArrayElement
        {
            get
            {
                if (_isArrayElement == null)
                {
                    if (ValueAccessor == null || !ValueAccessor.OwnerType.IsImplementsOpenGenericType(typeof(ICollection<>)))
                    {
                        _isArrayElement = false;
                    }
                    else
                    {
                        _isArrayElement = true;
                        //TODO IsArrayElement其他情况的补充
                    }
                }
                return _isArrayElement.Value;
            }
        }

        public static InspectorPropertyInfo CreateForUnityProperty(
            SerializedProperty serializedProperty,
            Type parentType, Type valueType)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                PropertyName = serializedProperty.name,
                IsUnityProperty = true,
                PropertyResolverLocator = new UnityPropertyResolverLocator()
            };

            if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                Assert.IsTrue(serializedProperty.isArray);
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                var accessorType = typeof(UnityCollectionAccessor<,,>)
                    .MakeGenericType(parentType, valueType, elementType);
                info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);
            }
            else
            {
                try
                {
                    var accessorType = typeof(UnityPropertyAccessor<,>)
                        .MakeGenericType(parentType, valueType);
                    info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);
                    info.ValueAccessor.GetWeakValue(FormatterServices.GetUninitializedObject(parentType));
                }
                catch (Exception e) //TODO 有的类型无法通过SerializedProperty获取
                {
                    info.ValueAccessor = null;
                }
            }

            return info;
        }

        public static InspectorPropertyInfo CreateForMember(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return CreateForField(fieldInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                return CreateForProperty(propertyInfo);
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                return CreateForMethod(methodInfo);
            }
            throw new NotSupportedException($"Unsupported member type: {memberInfo.GetType()}");
        }

        public static InspectorPropertyInfo CreateForProperty(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        public static InspectorPropertyInfo CreateForMethod(MethodInfo methodInfo)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyName = methodInfo.Name + "()",
                IsUnityProperty = false,
                _memberInfo = methodInfo
            };

            return info;
        }

        public static InspectorPropertyInfo CreateForField(FieldInfo fieldInfo)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = fieldInfo.FieldType,
                PropertyName = fieldInfo.Name,
                IsUnityProperty = false,
                _memberInfo = fieldInfo,
                PropertyResolverLocator = new GenericPropertyResolverLocator()
            };

            var accessorType = typeof(MemberValueAccessor<,>)
                .MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
            info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(fieldInfo);

            return info;
        }

        public static InspectorPropertyInfo CreateForValue(Type valueType, string valueName, IValueAccessor valueAccessor)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                PropertyName = valueName,
                IsUnityProperty = false,
                ValueAccessor = valueAccessor,
                PropertyResolverLocator = new GenericPropertyResolverLocator()
            };

            return info;
        }

        internal static InspectorPropertyInfo CreateForLogicRoot(SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();

            var info = new InspectorPropertyInfo()
            {
                PropertyType = serializedObject.targetObject.GetType(),
                PropertyName = "$ROOT$",
                IsLogicRoot = true,
                PropertyResolverLocator = new GenericPropertyResolverLocator()
            };

            info.ValueAccessor = new GenericValueAccessor(
                typeof(int),
                info.PropertyType,
                (ref object index) => serializedObject.targetObjects[(int)index],
                null);

            return info;
        }

        public MemberInfo GetMemberInfo()
        {
            if (TryGetMemberInfo(out var memberInfo))
            {
                return memberInfo;
            }

            throw new NotSupportedException($"Get member info failed for '{PropertyName}'.");
        }

        public bool TryGetMemberInfo(out MemberInfo memberInfo)
        {
            memberInfo = null;
            if (_memberInfo != null)
            {
                memberInfo = _memberInfo;
                return true;
            }

            if (ValueAccessor == null ||
                IsArrayElement ||
                PropertyName.IsNullOrEmpty())
            {
                return false;
            }

            var parentType = ValueAccessor.OwnerType;
            var fieldInfo = parentType.GetField(PropertyName, BindingFlagsHelper.AllInstance);
            if (fieldInfo != null)
            {
                memberInfo = fieldInfo;
                return true;
            }

            var propertyInfo = parentType.GetProperty(PropertyName, BindingFlagsHelper.AllInstance);
            if (propertyInfo != null)
            {
                memberInfo = propertyInfo;
                return true;
            }

            return false;
        }
    }
}

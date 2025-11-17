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
    /// <summary>
    /// Represents information about a property that can be inspected in the Unity editor.
    /// This class provides metadata and access capabilities for various types of properties
    /// including Unity serialized properties, fields, methods, and custom values.
    /// </summary>
    public sealed class InspectorPropertyInfo
    {
        private MemberInfo _memberInfo;
        private bool? _isArrayElement;

        /// <summary>
        /// Gets the property resolver locator used to find appropriate property resolvers for this property.
        /// </summary>
        [CanBeNull] public IPropertyResolverLocator PropertyResolverLocator { get; private set; }

        /// <summary>
        /// Gets the value accessor that provides read/write access to the property's value.
        /// </summary>
        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }

        /// <summary>
        /// Gets the type of the property value.
        /// </summary>
        [CanBeNull] public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property represents a logic root in the inspector hierarchy.
        /// </summary>
        public bool IsLogicRoot { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property is a Unity serialized property.
        /// </summary>
        public bool IsUnityProperty { get; private set; }

        private InspectorPropertyInfo()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this property represents an array element.
        /// This is determined by checking if the property's owner type implements <see cref="ICollection{T}"/>.
        /// </summary>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a Unity serialized property.
        /// </summary>
        /// <param name="serializedProperty">The Unity serialized property to create info for.</param>
        /// <param name="parentType">The type of the parent object containing this property.</param>
        /// <param name="valueType">The type of the property value.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the Unity serialized property.</returns>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a member (field, property, or method).
        /// </summary>
        /// <param name="memberInfo">The member info to create property info for.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the member.</returns>
        /// <exception cref="NotSupportedException">Thrown when the member type is not supported.</exception>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a property.
        /// </summary>
        /// <param name="propertyInfo">The property info to create property info for.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the property.</returns>
        /// <remarks>This method is not yet implemented.</remarks>
        public static InspectorPropertyInfo CreateForProperty(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a method.
        /// </summary>
        /// <param name="methodInfo">The method info to create property info for.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the method.</returns>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a field.
        /// </summary>
        /// <param name="fieldInfo">The field info to create property info for.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the field.</returns>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a custom value with a specified accessor.
        /// </summary>
        /// <param name="valueType">The type of the property value.</param>
        /// <param name="valueName">The name of the property.</param>
        /// <param name="valueAccessor">The value accessor that provides read/write access to the value.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the custom value.</returns>
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

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a logic root in the inspector hierarchy.
        /// This represents the root object being inspected.
        /// </summary>
        /// <param name="serializedObject">The serialized object representing the root.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured as a logic root.</returns>
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

        /// <summary>
        /// Gets the member information for this property.
        /// </summary>
        /// <returns>The member info associated with this property.</returns>
        /// <exception cref="NotSupportedException">Thrown when member info cannot be retrieved.</exception>
        public MemberInfo GetMemberInfo()
        {
            if (TryGetMemberInfo(out var memberInfo))
            {
                return memberInfo;
            }

            throw new NotSupportedException($"Get member info failed for '{PropertyName}'.");
        }

        /// <summary>
        /// Attempts to get the member information for this property.
        /// </summary>
        /// <param name="memberInfo">When this method returns, contains the member info if found; otherwise, null.</param>
        /// <returns>true if member info was found; otherwise, false.</returns>
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

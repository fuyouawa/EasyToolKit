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

        public Type OwnerType { get; private set; }

        /// <summary>
        /// Gets the type of the property value.
        /// </summary>
        public Type PropertyType { get; private set; }

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
                    if (OwnerType.IsImplementsOpenGenericType(typeof(ICollection<>)))
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
                OwnerType = parentType,
                PropertyName = serializedProperty.name,
                IsUnityProperty = true
            };

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
        public static InspectorPropertyInfo CreateForProperty(PropertyInfo propertyInfo)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = propertyInfo.PropertyType,
                OwnerType = propertyInfo.DeclaringType,
                PropertyName = propertyInfo.Name,
                IsUnityProperty = false,
                _memberInfo = propertyInfo
            };

            return info;
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
                OwnerType = methodInfo.DeclaringType,
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
                OwnerType = fieldInfo.DeclaringType,
                PropertyName = fieldInfo.Name,
                IsUnityProperty = false,
                _memberInfo = fieldInfo
            };

            return info;
        }

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a custom value.
        /// </summary>
        /// <param name="valueType">The type of the property value.</param>
        /// <param name="valueName">The name of the property.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the custom value.</returns>
        public static InspectorPropertyInfo CreateForValue(Type valueType, string valueName)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                OwnerType = valueType.DeclaringType,
                PropertyName = valueName,
                IsUnityProperty = false
            };

            return info;
        }

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a logic root in the inspector hierarchy.
        /// This represents the root object being inspected.
        /// </summary>
        /// <param name="targets">The target objects representing the root.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured as a logic root.</returns>
        internal static InspectorPropertyInfo CreateForLogicRoot(IList targets)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = targets[0].GetType(),
                PropertyName = "$ROOT$",
                IsLogicRoot = true
            };

            return info;
        }

        /// <summary>
        /// Creates an <see cref="InspectorPropertyInfo"/> for a collection element.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="elementIndex">The index of the element in the collection.</param>
        /// <returns>A new <see cref="InspectorPropertyInfo"/> instance configured for the collection element.</returns>
        internal static InspectorPropertyInfo CreateForCollectionElement(Type elementType, string elementName, int elementIndex, Type collectionType)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = elementType,
                PropertyName = elementName,
                OwnerType = collectionType,
                IsUnityProperty = false,
                _isArrayElement = true
            };

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

            if (IsArrayElement ||
                PropertyName.IsNullOrEmpty())
            {
                return false;
            }

            var fieldInfo = OwnerType.GetField(PropertyName, BindingFlagsHelper.AllInstance);
            if (fieldInfo != null)
            {
                memberInfo = fieldInfo;
                return true;
            }

            var propertyInfo = OwnerType.GetProperty(PropertyName, BindingFlagsHelper.AllInstance);
            if (propertyInfo != null)
            {
                memberInfo = propertyInfo;
                return true;
            }

            return false;
        }
    }
}

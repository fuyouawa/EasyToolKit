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

        public MemberInfo MemberInfo { get; private set; }
        public bool IsCollectionElement { get; private set; }
        public int CollectionElementIndex { get; private set; }

        private InspectorPropertyInfo()
        {
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
                MemberInfo = propertyInfo
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
                MemberInfo = methodInfo
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
                MemberInfo = fieldInfo
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
                IsCollectionElement = true,
                CollectionElementIndex = elementIndex
            };

            return info;
        }
    }
}

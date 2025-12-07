using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Utility class providing helper methods for inspecting and determining property information
    /// related to Unity serialization and inspector display.
    /// </summary>
    public static class InspectorPropertyInfoUtility
    {
        /// <summary>
        /// Determines if a type is satisfied of unity property drawer.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is satisfied of unity property drawer.; otherwise false.</returns>
        public static bool IsUnityPropertyDrawerSatisfiedBy(Type type)
        {
            if (IsEasyValueDrawerSatisfiedBy(type))
            {
                return false;
            }

            return InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(type);
        }

        /// <summary>
        /// Determines if a type is satisfied of easy value drawer.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is satisfied of easy value drawer.; otherwise false.</returns>
        public static bool IsEasyValueDrawerSatisfiedBy(Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return false;
            }

            return InspectorDrawerUtility.IsDefinedEasyValueDrawer(type);
        }

        /// <summary>
        /// Determines if a type is serializable by Unity's serialization system.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is serializable; otherwise false.</returns>
        public static bool IsSerializableType(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
                return false;
            return type.IsBasicValueType() ||
                   type.IsUnityBuiltInType() ||
                   type.IsInheritsFrom<UnityEngine.Object>() ||
                   type.IsDefined<SerializableAttribute>();
        }

        /// <summary>
        /// Determines if a field is serializable by Unity's serialization system.
        /// </summary>
        /// <param name="fieldInfo">The field to check.</param>
        /// <returns>True if the field is serializable; otherwise false.</returns>
        public static bool IsSerializableField(FieldInfo fieldInfo)
        {
            if (!IsSerializableType(fieldInfo.FieldType))
            {
                return false;
            }

            var nonSerialized = fieldInfo.IsDefined<NonSerializedAttribute>();
            if (fieldInfo.IsPublic && !nonSerialized)
            {
                return true;
            }

            return !nonSerialized && fieldInfo.IsDefined<SerializeField>();
        }

        /// <summary>
        /// Determines if a type can have child properties in the inspector.
        /// Uses lenient criteria that excludes basic value types, delegates, and Unity Object types.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type can have children; otherwise false.</returns>
        public static bool IsAllowChildrenTypeLeniently(Type type)
        {
            return !type.IsBasicValueType() &&
                   !typeof(Delegate).IsAssignableFrom(type) &&
                   !IsUnityPropertyDrawerSatisfiedBy(type);
        }

        // public static bool IsAllowChildrenField(FieldInfo fieldInfo)
        // {
        //     if (!IsAllowChildrenTypeLeniently(fieldInfo.FieldType))
        //     {
        //         return false;
        //     }
        //
        //     if (fieldInfo.IsDefined<ShowInInspectorAttribute>())
        //     {
        //         return true;
        //     }
        //
        //     return IsSerializableField(fieldInfo);
        // }
    }
}

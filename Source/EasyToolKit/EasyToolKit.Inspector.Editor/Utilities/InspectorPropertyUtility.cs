using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyUtility
    {
        /// <summary>
        /// Determines if a type is satisfied of unity property drawer.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is satisfied of unity property drawer.; otherwise false.</returns>
        public static bool IsUnityPropertyDrawerSatisfiedBy(Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return false;
            }

            return InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(type);
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
    }
}

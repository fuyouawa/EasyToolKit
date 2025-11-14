using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyInfoUtility
    {
        public static bool IsUnityObjectTypeOrDefinedUnityPropertyDrawer(Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return true;
            }
            var isDefinedUnityPropertyDrawer = InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(type);
            return isDefinedUnityPropertyDrawer;
        }

        public static bool IsSerializableType(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
                return false;
            return type.IsBasicValueType() ||
                   type.IsUnityBuiltInType() ||
                   type.IsInheritsFrom<UnityEngine.Object>() ||
                   type.IsDefined<SerializableAttribute>();
        }

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

        public static bool IsAllowChildrenTypeLeniently(Type type)
        {
            return !type.IsBasicValueType() &&
                   !typeof(Delegate).IsAssignableFrom(type) &&
                !IsUnityObjectTypeOrDefinedUnityPropertyDrawer(type);
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

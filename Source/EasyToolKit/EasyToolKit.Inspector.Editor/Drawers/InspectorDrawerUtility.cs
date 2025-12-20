using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorDrawerUtility
    {
        private static readonly Dictionary<Type, Type> UnityPropertyDrawerTypesByDrawnType =
            new Dictionary<Type, Type>();

        private static readonly FieldInfo CustomPropertyDrawerTypeFieldInfo =
            typeof(UnityEditor.CustomPropertyDrawer).GetField("m_Type", BindingFlagsHelper.AllInstance);

        static InspectorDrawerUtility()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(asm => asm.GetTypes())
                         .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && !t.IsInheritsFrom<UnityEditor.PropertyDrawer>()))
            {
                var customPropertyDrawers = type.GetCustomAttributes<UnityEditor.CustomPropertyDrawer>();
                foreach (var drawer in customPropertyDrawers)
                {
                    var drawnType = (Type)CustomPropertyDrawerTypeFieldInfo.GetValue(drawer);
                    if (drawnType != null)
                    {
                        UnityPropertyDrawerTypesByDrawnType[drawnType] = type;
                    }
                }
            }

            InspectorElementUtility.AddNullPriorityFallback(type =>
            {
                if (type.IsImplementsOpenGenericType(typeof(EasyAttributeDrawer<>)))
                {
                    return DrawerPriorityAttribute.AttributePriority;
                }
                if (type.IsImplementsOpenGenericType(typeof(EasyValueDrawer<>)))
                {
                    return DrawerPriorityAttribute.ValuePriority;
                }

                return null;
            });
        }

        public static bool IsDefinedUnityPropertyDrawer(Type targetType)
        {
            if (UnityPropertyDrawerTypesByDrawnType.ContainsKey(targetType))
            {
                return true;
            }

            //TODO 通过CustomPropertyDrawer判断是否需要检查继承
            foreach (var drawnType in UnityPropertyDrawerTypesByDrawnType.Keys)
            {
                if (drawnType.IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<Type> GetDrawerTypes(InspectorProperty property)
        {
            var additionalMatchTypesList = new List<Type[]>();
            foreach (var attribute in property.GetAttributes())
            {
                additionalMatchTypesList.Add(new[] { attribute.GetType() });
                if (property.ValueEntry != null)
                {
                    additionalMatchTypesList.Add(new[] { attribute.GetType(), property.ValueEntry.ValueType });
                }
            }

            return InspectorElementUtility.GetElementTypes(property, type => type.IsInheritsFrom<IEasyDrawer>(), additionalMatchTypesList);
        }
    }
}

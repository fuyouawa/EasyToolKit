using System;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value - 1)]
    public class UnityPropertyDrawer : EasyDrawer
    {
        protected override bool CanDrawProperty(InspectorProperty property)
        {
            // if (!property.Info.IsUnityProperty)
            // {
            //     return false;
            // }

            if (property.Tree.SerializedObject == null)
            {
                return false;
            }

            var propertyType = property.Info.PropertyType;
            var unityProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);
            if (propertyType == null || unityProperty == null)
            {
                return false;
            }

            return !propertyType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                   InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(propertyType);
        }

        private SerializedProperty _serializedProperty;

        protected override void Initialize()
        {
            _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.UnityPath);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_serializedProperty == null)
            {
                EditorGUILayout.LabelField(label, EditorHelper.TempContent("Not Supported"));
                return;
            }

            EditorGUILayout.PropertyField(_serializedProperty, label);
        }
    }
}

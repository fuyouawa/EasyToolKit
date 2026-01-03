using System;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 0.2)]
    public class UnityPropertyDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawElement(IValueElement element)
        {
            if (element.SharedContext.Tree.SerializedObject == null)
            {
                return false;
            }

            if (element is IFieldElement fieldElement)
            {
                var propertyType = fieldElement.Definition.ValueType;
                var unityProperty = fieldElement.SharedContext.Tree.GetUnityPropertyByPath(fieldElement.UnityPath);
                if (propertyType == null || unityProperty == null)
                {
                    return false;
                }

                return !propertyType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                       InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(propertyType);
            }

            return false;
        }

        private SerializedProperty _serializedProperty;

        protected override void Initialize()
        {
            var fieldElement = (IFieldElement)Element;
            _serializedProperty = fieldElement.SharedContext.Tree.GetUnityPropertyByPath(fieldElement.UnityPath);
        }

        protected override void Draw(GUIContent label)
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

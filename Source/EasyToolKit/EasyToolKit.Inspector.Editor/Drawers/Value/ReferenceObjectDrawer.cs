using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 0.1)]
    public class ReferenceObjectDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            var isUnityBuiltInType = property.ValueEntry.ValueType.IsUnityBuiltInType();
            if (isUnityBuiltInType)
            {
                return false;
            }

            if (property.Children == null)
            {
                return InspectorPropertyInfoUtility.IsAllowChildrenTypeLeniently(property.ValueEntry.ValueType);
            }
            return true;
        }

        private ReferenceObjectDrawerSettingsAttribute _settings;

        protected override void Initialize()
        {
            _settings = Property.GetAttribute<ReferenceObjectDrawerSettingsAttribute>();
        }

        protected override void DrawProperty(GUIContent label)
        {
            var containsNull = ValueEntry.Values.Any(value => value == null);

            if (containsNull)
            {
                var instantiateIfNull = InspectorConfigAsset.Instance.TryInstantiateReferenceObjectIfNull || _settings?.InstantiateIfNull == true;

                if (instantiateIfNull)
                {
                    if (ValueEntry.ValueType.IsInstantiable())
                    {
                        InstantiateIfNull();
                        containsNull = false;
                    }
                    else
                    {
                        if (_settings?.InstantiateIfNull == true)
                        {
                            EasyEditorGUI.MessageBox($"Type {ValueEntry.ValueType} is not instiatable.", MessageType.Error);
                            return;
                        }
                    }
                }
            }

            if (containsNull)
            {
                if (_settings?.HideIfNull == true)
                {
                    return;
                }

                if (label == null)
                {
                    EditorGUILayout.LabelField("Null");
                }
                else
                {
                    EditorGUILayout.LabelField(label, EditorHelper.TempContent("Null"));
                }
                return;
            }

            if (label == null)
            {
                CallNextDrawer(null);
                return;
            }

            if (_settings?.HideFoldout == true || Property.ChildrenResolver is ICollectionStructureResolver)
            {
                CallNextDrawer(label);
                return;
            }

            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, label);

            if (Property.State.Expanded)
            {
                EditorGUI.indentLevel++;
                CallNextDrawer(null);
                EditorGUI.indentLevel--;
            }
        }

        private void InstantiateIfNull()
        {
            for (int i = 0; i < ValueEntry.Values.Count; i++)
            {
                if (ValueEntry.Values[i] == null)
                {
                    ValueEntry.Values[i] = ValueEntry.ValueType.CreateInstance<T>();
                }
            }
        }
    }
}

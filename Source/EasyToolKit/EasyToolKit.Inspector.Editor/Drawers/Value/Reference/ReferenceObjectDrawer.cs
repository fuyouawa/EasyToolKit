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
        protected override bool CanDrawElement(IValueElement element)
        {
            var isUnityBuiltInType = element.ValueEntry.ValueType.IsUnityBuiltInType();
            if (isUnityBuiltInType)
            {
                return false;
            }

            return element.Children != null;
        }

        private ReferenceObjectDrawerSettingsAttribute _settings;

        protected override void Initialize()
        {
            _settings = Element.GetAttribute<ReferenceObjectDrawerSettingsAttribute>();
        }

        protected override void Draw(GUIContent label)
        {
            var containsNull = false;
            for (int i = 0; i < ValueEntry.TargetCount; i++)
            {
                if (ValueEntry.GetValue(i) == null)
                {
                    containsNull = true;
                    break;
                }
            }

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

            if (_settings?.HideFoldout == true || Element.Definition.Roles.IsCollection())
            {
                CallNextDrawer(label);
                return;
            }

            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, label);

            if (Element.State.Expanded)
            {
                EditorGUI.indentLevel++;
                CallNextDrawer(null);
                EditorGUI.indentLevel--;
            }
        }

        private void InstantiateIfNull()
        {
            for (int i = 0; i < ValueEntry.TargetCount; i++)
            {
                if (ValueEntry.GetValue(i) == null)
                {
                    ValueEntry.SetValue(i, ValueEntry.ValueType.CreateInstance<T>());
                }
            }
        }
    }
}

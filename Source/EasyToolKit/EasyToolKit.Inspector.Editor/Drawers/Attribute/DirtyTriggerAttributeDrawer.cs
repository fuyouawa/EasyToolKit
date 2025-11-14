using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class DirtyTriggerAttributeDrawer : EasyAttributeDrawer<DirtyTriggerAttribute>
    {
        private string _errorMessage;

        protected override void Initialize()
        {
            if (Property.ValueEntry.ValueType != typeof(Action<string>))
            {
                _errorMessage = $"The dirty property '{Property.Path}' must be a Action or Action<string>!";
                return;
            }

            for (int i = 0; i < Property.ValueEntry.ValueCount; i++)
            {
                var action = (Action<string>)Property.ValueEntry.WeakValues[i];
                if (action is null)
                {
                    action = OnDirtyPropertyTriggered;
                }
                else
                {
                    action += OnDirtyPropertyTriggered;
                }
                Property.ValueEntry.WeakValues[i] = action;
            }
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_errorMessage != null)
            {
                EasyEditorGUI.MessageBox(_errorMessage, MessageType.Error);
            }

            CallNextDrawer(label);
        }


        private void OnDirtyPropertyTriggered(string propertyName)
        {
            if (propertyName.IsNotNullOrWhiteSpace())
            {
                var dirtyProperty = Property.Parent.Children[propertyName];
                dirtyProperty.ValueEntry.WeakValues.ForceMakeDirty();
            }
            else
            {
                Property.Parent.ValueEntry.WeakValues.ForceMakeDirty();
            }
        }
    }
}
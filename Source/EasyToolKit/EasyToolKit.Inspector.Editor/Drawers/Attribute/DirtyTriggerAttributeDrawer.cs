using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class DirtyTriggerAttributeDrawer<T> : EasyAttributeDrawer<DirtyTriggerAttribute, T>
        where T : Delegate
    {
        private string _errorMessage;

        protected override void Initialize()
        {
            if (ValueEntry.ValueType != typeof(Action<string>))
            {
                _errorMessage = $"The dirty property '{Element.Path}' must be a Action or Action<string>!";
                return;
            }

            for (int i = 0; i < ValueEntry.TargetCount; i++)
            {
                var action = (Action<string>)ValueEntry.GetWeakValue(i);
                if (action is null)
                {
                    action = OnDirtyPropertyTriggered;
                }
                else
                {
                    action += OnDirtyPropertyTriggered;
                }
                ValueEntry.SetWeakValue(i, action);
            }
        }

        protected override void Draw(GUIContent label)
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
                var dirtyElement = Element.LogicalParent!.Children![propertyName];
                if (dirtyElement is IValueElement dirtyValueElement)
                {
                    dirtyValueElement.ValueEntry.MarkDirty();
                }
            }
            else
            {
                Element.LogicalParent.CastValue().ValueEntry.MarkDirty();
            }
        }
    }
}

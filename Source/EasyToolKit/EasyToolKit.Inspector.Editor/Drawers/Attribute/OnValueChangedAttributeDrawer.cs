using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class OnValueChangedAttributeDrawer : EasyAttributeDrawer<OnValueChangedAttribute>
    {
        private MethodInfo _methodInfo;
        private string _error;

        protected override void DrawProperty(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_methodInfo == null)
            {
                var targetType = this.GetTargetTypeForResolver();
                try
                {
                    _methodInfo = targetType.GetMethodEx(Attribute.Method, BindingFlagsHelper.All, Property.ValueEntry.ValueType);

                    Property.ValueEntry.OnValueChanged += OnValueChanged;
                }
                catch (Exception e)
                {
                    _error = e.Message;
                }
            }

            CallNextDrawer(label);
        }

        private void OnValueChanged(int targetIndex)
        {
            var target = this.GetTargetForResolver(targetIndex);
            var value = Property.ValueEntry.WeakValues[targetIndex];
            if (_methodInfo.IsStatic)
            {
                _methodInfo.Invoke(null, new object[] { value });
            }
            else
            {
                _methodInfo.Invoke(target, new object[] { value });
            }
        }
    }
}
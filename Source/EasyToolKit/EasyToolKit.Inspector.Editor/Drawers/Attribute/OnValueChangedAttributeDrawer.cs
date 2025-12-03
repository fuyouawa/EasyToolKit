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
                    try
                    {
                        _methodInfo = targetType.GetMethodEx(Attribute.Method, BindingFlagsHelper.All, Property.ValueEntry.ValueType);
                    }
                    catch (Exception e)
                    {
                        _methodInfo = targetType.GetMethodEx(Attribute.Method, BindingFlagsHelper.All);
                    }

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
            var value = Property.ValueEntry.WeakValues[targetIndex];
            var args = _methodInfo.GetParameters().Length == 0 ? null : new object[] { value };
            if (_methodInfo.IsStatic)
            {
                _methodInfo.Invoke(null, args);
            }
            else
            {
                var target = this.GetTargetForResolver(targetIndex);
                _methodInfo.Invoke(target, args);
            }
        }
    }
}

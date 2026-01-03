using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class OnValueChangedAttributeDrawer<T> : EasyAttributeDrawer<OnValueChangedAttribute, T>
    {
        private MethodInfo _methodInfo;
        private string _error;

        protected override void Draw(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_methodInfo == null)
            {
                var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);
                try
                {
                    try
                    {
                        _methodInfo = targetType.GetMethodEx(Attribute.Method, BindingFlagsHelper.All, ValueEntry.ValueType);
                    }
                    catch (Exception e)
                    {
                        _methodInfo = targetType.GetMethodEx(Attribute.Method, BindingFlagsHelper.All);
                    }

                    ValueEntry.AfterValueChanged += OnValueChanged;
                }
                catch (Exception e)
                {
                    _error = e.Message;
                }
            }

            CallNextDrawer(label);
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs eventArgs)
        {
            var value = ValueEntry.GetWeakValue(eventArgs.TargetIndex);
            var args = _methodInfo.GetParameters().Length == 0 ? null : new object[] { value };
            if (_methodInfo.IsStatic)
            {
                _methodInfo.Invoke(null, args);
            }
            else
            {
                var target = ElementUtility.GetOwnerWithAttribute(Element, Attribute, eventArgs.TargetIndex);
                _methodInfo.Invoke(target, args);
            }
        }
    }
}

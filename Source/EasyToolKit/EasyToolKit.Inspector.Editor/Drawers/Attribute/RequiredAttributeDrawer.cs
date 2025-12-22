using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class RequiredAttributeDrawer : EasyAttributeDrawer<RequiredAttribute>
    {
        private bool _isValueType;
        private bool _isIntegerType;
        private bool _isFloatingPointType;
        private bool _isStringType;
        private bool _isUnityObjectType;
        private bool _isEnumType;
        private bool _isNotRequireType;

        private string _zeroEnumName;

        protected override void Initialize()
        {
            var valueType = Property.ValueEntry.ValueType;

            _isValueType = valueType.IsValueType;
            _isIntegerType = valueType.IsIntegerType();
            _isFloatingPointType = valueType.IsFloatingPointType();
            _isUnityObjectType = valueType.IsUnityObjectType();
            _isStringType = valueType.IsStringType();
            _isEnumType = valueType.IsEnum;
            _isNotRequireType = _isValueType &&
                                !_isIntegerType &&
                                !_isFloatingPointType;

            if (_isEnumType)
            {
                var enumValues = Enum.GetValues(valueType);
                foreach (var value in enumValues)
                {
                    if ((long)value == 0)
                    {
                        _zeroEnumName = Enum.GetName(valueType, value);
                        break;
                    }
                }
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_isNotRequireType)
            {
                EasyEditorGUI.MessageBox($"{label.text} is no need to use RequireAttribute.", MessageType.Warning);
                CallNextDrawer(label);
                return;
            }

            var require = false;
            for (int i = 0; i < Property.ValueEntry.ValueCount; i++)
            {
                var value = Property.ValueEntry.WeakValues[i];
                if (_isStringType)
                {
                    if (string.IsNullOrEmpty((string)value))
                    {
                        require = true;
                        break;
                    }
                }
                else if (_isValueType)
                {
                    if (_isIntegerType || _isEnumType)
                    {
                        if ((long)value == 0)
                        {
                            require = true;
                            break;
                        }
                    }
                    else if (_isFloatingPointType)
                    {
                        if (((double)value).IsApproximatelyOf(0.0))
                        {
                            require = true;
                            break;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(Property.ValueEntry.ValueType.ToString());
                    }
                }
                else
                {
                    if (_isUnityObjectType)
                    {
                        if ((UnityEngine.Object)value == null)
                        {
                            require = true;
                            break;
                        }
                    }
                    if (value == null)
                    {
                        require = true;
                        break;
                    }
                }
            }

            if (require)
            {
                string message;
                if (_isValueType)
                {
                    if (_isIntegerType || _isFloatingPointType)
                    {
                        message = $"{label.text} can not be zero.";
                    }
                    else if (_isEnumType)
                    {
                        message = $"{label.text} can not be {_zeroEnumName ?? "zero"}.";
                    }
                    else
                    {
                        message = $"{label.text} can not be empty.";
                    }
                }
                else
                {
                    message = $"{label.text} can not be null.";
                }
                EasyEditorGUI.MessageBox(message, MessageType.Error);
            }

            CallNextDrawer(label);
        }
    }
}

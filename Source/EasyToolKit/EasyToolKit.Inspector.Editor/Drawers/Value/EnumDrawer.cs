using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EnumDrawer<T> : EasyValueDrawer<T>
    {
        private static T[] s_enumValues;
        private static T[] EnumValues => s_enumValues ??= Enum.GetValues(typeof(T)) as T[];

        private static GUIContent[] s_enumContents;
        private static GUIContent[] EnumContents
        {
            get
            {
                if (s_enumContents == null)
                {
                    var enumType = typeof(T);
                    var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

                    var nameToLabel = fields.ToDictionary(
                        fieldInfo => fieldInfo.Name,
                        fieldInfo =>
                        {
                            var labelAttr = fieldInfo.GetCustomAttribute<LabelTextAttribute>();
                            return labelAttr?.Label ?? fieldInfo.Name;
                        });

                    var values = EnumValues ?? Array.Empty<T>();
                    s_enumContents = values
                        .Select(v =>
                        {
                            var name = Enum.GetName(enumType, v) ?? v?.ToString() ?? string.Empty;
                            var text = nameToLabel.GetValueOrDefault(name, name);
                            return new GUIContent(text);
                        })
                        .ToArray();
                }
                return s_enumContents;
            }
        }

        protected override bool CanDrawValueType(Type valueType)
        {
            return valueType.IsEnum;
        }

        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (value.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                value = (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)value);
            }
            else
            {
                value = EasyEditorGUI.ValueDropdown(label, value, EnumValues, EnumContents);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (!value.Equals(ValueEntry.SmartValue))
                {
                    ValueEntry.SmartValue = value;
                }
            }
        }
    }
}

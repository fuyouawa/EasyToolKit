using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeInt8Drawer : EasyAttributeDrawer<RangeAttribute, byte>
    {
        protected override void Draw(GUIContent label)
        {
            var min = (int)Attribute.min;
            var max = (int)Attribute.max;
            var value = (int)ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.IntSlider(value, min, max);
            }
            else
            {
                value = EditorGUILayout.IntSlider(label, value, min, max);
            }

            // 确保值在byte范围内
            if (value < byte.MinValue)
            {
                value = byte.MinValue;
            }
            else if (value > byte.MaxValue)
            {
                value = byte.MaxValue;
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (byte)value;
            }
        }
    }
}

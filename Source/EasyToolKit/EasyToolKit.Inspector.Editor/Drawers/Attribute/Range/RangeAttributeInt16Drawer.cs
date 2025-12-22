using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeInt16Drawer : EasyAttributeDrawer<RangeAttribute, short>
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

            // 确保值在short范围内
            if (value < short.MinValue)
            {
                value = short.MinValue;
            }
            else if (value > short.MaxValue)
            {
                value = short.MaxValue;
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (short)value;
            }
        }
    }
}

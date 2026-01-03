using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeInt64Drawer : EasyAttributeDrawer<RangeAttribute, long>
    {
        protected override void Draw(GUIContent label)
        {
            var min = (int)Attribute.min;
            var max = (int)Attribute.max;
            var value = ValueEntry.SmartValue;
            if (value > int.MaxValue)
            {
                value = int.MaxValue;
            }
            else if (value < int.MinValue)
            {
                value = int.MinValue;
            }

            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.IntSlider((int)value, min, max);
            }
            else
            {
                value = EditorGUILayout.IntSlider(label, (int)value, min, max);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (long)value;
            }
        }
    }
}

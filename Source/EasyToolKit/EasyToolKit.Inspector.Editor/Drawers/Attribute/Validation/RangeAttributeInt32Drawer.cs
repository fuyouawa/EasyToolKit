using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeInt32Drawer : EasyAttributeDrawer<RangeAttribute, int>
    {
        protected override void Draw(GUIContent label)
        {
            var min = (int)Attribute.min;
            var max = (int)Attribute.max;
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.IntSlider(value, min, max);
            }
            else
            {
                value = EditorGUILayout.IntSlider(label, value, min, max);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

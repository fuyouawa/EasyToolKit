using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeFloatDrawer : EasyAttributeDrawer<RangeAttribute, float>
    {
        protected override void Draw(GUIContent label)
        {
            var min = Attribute.min;
            var max = Attribute.max;
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.Slider(value, min, max);
            }
            else
            {
                value = EditorGUILayout.Slider(label, value, min, max);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

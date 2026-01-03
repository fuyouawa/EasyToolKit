using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class RangeAttributeDoubleDrawer : EasyAttributeDrawer<RangeAttribute, double>
    {
        protected override void Draw(GUIContent label)
        {
            var min = (double)Attribute.min;
            var max = (double)Attribute.max;
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();
            
            // Unity的Slider不支持double类型，所以我们需要手动实现
            float floatValue = (float)value;
            float floatMin = (float)min;
            float floatMax = (float)max;
            
            if (label == null)
            {
                floatValue = EditorGUILayout.Slider(floatValue, floatMin, floatMax);
            }
            else
            {
                floatValue = EditorGUILayout.Slider(label, floatValue, floatMin, floatMax);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (double)floatValue;
            }
        }
    }
}

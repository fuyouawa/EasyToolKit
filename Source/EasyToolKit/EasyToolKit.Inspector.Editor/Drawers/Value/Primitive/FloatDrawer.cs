using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class FloatDrawer : EasyValueDrawer<float>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.FloatField(value);
            }
            else
            {
                value = EditorGUILayout.FloatField(label, value);
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

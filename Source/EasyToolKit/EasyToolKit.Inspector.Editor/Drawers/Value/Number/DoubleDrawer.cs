using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class DoubleDrawer : EasyValueDrawer<double>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.DoubleField(value);
            }
            else
            {
                value = EditorGUILayout.DoubleField(label, value);
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

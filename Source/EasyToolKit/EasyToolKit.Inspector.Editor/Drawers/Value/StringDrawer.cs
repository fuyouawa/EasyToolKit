using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class StringDrawer : EasyValueDrawer<string>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.TextField(value);
            }
            else
            {
                value = EditorGUILayout.TextField(label, value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

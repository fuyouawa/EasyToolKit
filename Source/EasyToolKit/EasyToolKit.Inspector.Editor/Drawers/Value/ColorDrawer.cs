using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class ColorDrawer : EasyValueDrawer<Color>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.ColorField(value);
            }
            else
            {
                value = EditorGUILayout.ColorField(label, value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

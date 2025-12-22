using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class BooleanDrawer : EasyValueDrawer<bool>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.Toggle(value);
            }
            else
            {
                value = EditorGUILayout.Toggle(label, value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

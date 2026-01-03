using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int16Drawer : EasyValueDrawer<short>
    {
        protected override void Draw(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            int value;
            if (label == null)
            {
                value = EditorGUILayout.IntField(ValueEntry.SmartValue);
            }
            else
            {
                value = EditorGUILayout.IntField(label, ValueEntry.SmartValue);
            }

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

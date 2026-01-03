using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int8Drawer : EasyValueDrawer<byte>
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

            if (value < byte.MinValue)
            {
                value = byte.MinValue;
            }
            else if (value > byte.MaxValue)
            {
                value = byte.MaxValue;
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (byte)value;
            }
        }
    }
}

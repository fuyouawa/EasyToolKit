using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int64Drawer : EasyValueDrawer<long>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                value = EditorGUILayout.LongField(value);
            }
            else
            {
                value = EditorGUILayout.LongField(label, value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}

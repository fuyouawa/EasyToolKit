using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 2)]
    public class GuidDrawer : EasyValueDrawer<Guid>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginDisabledGroup(true);
            if (label == null)
            {
                EditorGUILayout.TextField(value.ToString("D"));
            }
            else
            {
                EditorGUILayout.TextField(label, value.ToString("D"));
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value - 1)]
    public class GenericValueDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawElement(IValueElement element)
        {
            return element.Children != null;
        }

        protected override void Draw(GUIContent label)
        {
            if (label == null)
            {
                foreach (var child in Element.Children)
                {
                    child.Draw(child.Label);
                }
            }
            else
            {
                EditorGUI.indentLevel++;
                foreach (var child in Element.Children)
                {
                    child.Draw(child.Label);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}

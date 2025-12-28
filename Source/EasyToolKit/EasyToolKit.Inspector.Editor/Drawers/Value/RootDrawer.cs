using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute - 1)]
    public class RootDrawer<T> : EasyValueDrawer<T>
        where T : UnityEngine.Object
    {
        protected override bool CanDrawElement(IValueElement element)
        {
            return element.Definition.Roles.IsRoot();
        }

        protected override void Draw(GUIContent label)
        {
            foreach (var child in Element.Children)
            {
                child.Draw(child.Label);
            }
        }
    }
}

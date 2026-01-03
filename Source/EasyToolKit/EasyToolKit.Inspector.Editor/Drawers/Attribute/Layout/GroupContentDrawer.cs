using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value - 1)]
    public class GroupContentDrawer : EasyDrawer
    {
        protected override bool CanDraw(IElement element)
        {
            return element.Definition.Roles.IsGroup();
        }

        protected override void Draw(GUIContent label)
        {
            if (Element.State.Expanded)
            {
                foreach (var child in Element.Children)
                {
                    child.Draw(child.Label);
                }
            }
        }
    }
}

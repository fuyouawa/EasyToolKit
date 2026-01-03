using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class HideInInspectorAttributeDrawer : EasyAttributeDrawer<HideInInspector>
    {
        protected override void Draw(GUIContent label)
        {
        }
    }
}

using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class HideLabelAttributeDrawer : EasyAttributeDrawer<HideLabelAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            CallNextDrawer(null);
        }
    }
}

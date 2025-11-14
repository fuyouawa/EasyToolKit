using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class FolderPathAttributeDrawer : EasyAttributeDrawer<FolderPathAttribute>
    {
        protected override void DrawProperty(GUIContent label)
        {
            //TODO FolderPathAttributeDrawer
            CallNextDrawer(label);
        }
    }
}

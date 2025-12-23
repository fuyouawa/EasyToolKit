using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyGroupAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : BeginGroupAttribute
    {
        public new IGroupElement Element => base.Element as IGroupElement;

        protected override void Draw(GUIContent label)
        {
            bool foldout = true;
            BeginDrawGroup(label);
            CallNextDrawer(label);
            EndDrawGroup();
        }

        protected virtual void BeginDrawGroup(GUIContent label)
        {
        }

        protected virtual void EndDrawGroup()
        {
        }

        protected override bool CanDraw(IElement element)
        {
            if (element is IGroupElement groupElement)
            {
                return CanDrawElement(groupElement);
            }

            return false;
        }

        protected virtual bool CanDrawElement(IGroupElement element)
        {
            return true;
        }
    }
}

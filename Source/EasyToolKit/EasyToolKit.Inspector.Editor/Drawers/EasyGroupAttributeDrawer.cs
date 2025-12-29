using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyGroupAttributeDrawer<TAttribute> : EasyDrawer
        where TAttribute : BeginGroupAttribute
    {
        private TAttribute _attribute;
        public new IGroupElement Element => (IGroupElement)base.Element;

        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = Element.GetAttribute<TAttribute>();
                }

                return _attribute;
            }
        }

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

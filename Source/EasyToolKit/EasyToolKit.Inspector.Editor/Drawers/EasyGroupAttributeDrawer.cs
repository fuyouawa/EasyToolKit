using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyGroupAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : BeginGroupAttribute
    {
        private bool _previousFoldout = false;

        protected override void DrawProperty(GUIContent label)
        {
            bool foldout = true;
            BeginDrawProperty(label, ref foldout);
            if (Event.current.type == EventType.Layout)
            {
                _previousFoldout = foldout;
            }
            if (_previousFoldout)
            {
                CallNextDrawer(label);
                var groupProperties = Property.GetGroupProperties(typeof(TAttribute));
                foreach (var groupProperty in groupProperties)
                {
                    groupProperty.Draw();
                    groupProperty.SkipDrawCount++;
                }
            }
            else
            {
                var groupProperties = Property.GetGroupProperties(typeof(TAttribute));
                foreach (var groupProperty in groupProperties)
                {
                    groupProperty.SkipDrawCount++;
                }
            }

            EndDrawProperty();
        }

        protected virtual void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
        }

        protected virtual void EndDrawProperty()
        {
        }
    }
}

using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base abstract class for drawers that handle group attributes.
    /// Group attributes create collapsible sections in the Unity Inspector.
    /// </summary>
    /// <typeparam name="TAttribute">The type of group attribute this drawer handles.</typeparam>
    public abstract class EasyGroupAttributeDrawer<TAttribute> : EasyAttributeDrawer<TAttribute>
        where TAttribute : BeginGroupAttribute
    {
        private bool _previousFoldout = false;

        /// <summary>
        /// Draws the group property with foldout functionality.
        /// When the group is expanded, it draws the property and all properties in the group.
        /// When collapsed, it skips drawing the group properties.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
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

        /// <summary>
        /// Called at the beginning of the group drawing process.
        /// Override this method to customize the group header appearance.
        /// </summary>
        /// <param name="label">The GUIContent label for the group.</param>
        /// <param name="foldout">Reference to the foldout state. Modify this to control group expansion.</param>
        protected virtual void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
        }

        /// <summary>
        /// Called at the end of the group drawing process.
        /// Override this method to add custom UI after the group content.
        /// </summary>
        protected virtual void EndDrawProperty()
        {
        }
    }
}

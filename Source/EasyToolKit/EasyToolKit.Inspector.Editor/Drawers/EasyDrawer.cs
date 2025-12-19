using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Base abstract class for all EasyToolKit drawers.
    /// Provides common functionality for property drawing and lifecycle management.
    /// </summary>
    public abstract class EasyDrawer : InspectorElementBase, IEasyDrawer
    {
        /// <summary>
        /// Gets or sets whether this drawer should be skipped during the drawing process.
        /// </summary>
        public bool SkipWhenDrawing { get; set; }

        /// <summary>
        /// Determines whether this drawer can draw the specified property.
        /// Override this method to implement custom property filtering logic.
        /// </summary>
        /// <param name="property">The InspectorProperty to check.</param>
        /// <returns>True if this drawer can handle the property, false otherwise.</returns>
        protected virtual bool CanDrawProperty(InspectorProperty property)
        {
            return true;
        }

        /// <summary>
        /// Calls the next drawer in the drawer chain for the current property.
        /// This allows multiple drawers to process the same property in sequence.
        /// </summary>
        /// <param name="label">The <see cref="GUIContent"/> label to pass to the next drawer.</param>
        /// <returns>True if there was a next drawer that was called, false otherwise.</returns>
        protected bool CallNextDrawer(GUIContent label)
        {
            var chain = Property.GetDrawerChain();
            if (chain.MoveNext())
            {
                chain.Current.DrawProperty(label);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Main method for drawing the property UI.
        /// Override this method to implement custom drawing logic.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        protected virtual void DrawProperty(GUIContent label)
        {
        }

        /// <summary>
        /// Explicit interface implementation for IEasyDrawer.DrawProperty.
        /// Draws the property UI using the specified label.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        void IEasyDrawer.DrawProperty(GUIContent label)
        {
            DrawProperty(label);
        }

        protected override bool CanHandle(InspectorProperty property)
        {
            return CanDrawProperty(property);
        }
    }
}

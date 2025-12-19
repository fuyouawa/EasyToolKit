using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Interface for all EasyToolKit drawers that handle property drawing in the Unity Inspector.
    /// Drawers are responsible for rendering custom UI for properties and attributes.
    /// </summary>
    public interface IEasyDrawer : IInspectorElement
    {
        /// <summary>
        /// Gets or sets whether this drawer should be skipped during the drawing process.
        /// When true, the drawer will not render its UI.
        /// </summary>
        bool SkipWhenDrawing { get; set; }

        /// <summary>
        /// Draws the property UI using the specified label.
        /// </summary>
        /// <param name="label">The GUIContent label to display for this property.</param>
        void DrawProperty(GUIContent label);
    }
}

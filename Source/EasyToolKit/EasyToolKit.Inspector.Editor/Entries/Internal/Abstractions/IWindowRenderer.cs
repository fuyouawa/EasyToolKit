using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Handles the rendering of an EasyEditorWindow's GUI.
    /// </summary>
    internal interface IWindowRenderer
    {
        /// <summary>
        /// Gets the current content size.
        /// </summary>
        Vector2 ContentSize { get; }

        /// <summary>
        /// Gets the current scroll position.
        /// </summary>
        Vector2 ScrollPosition { get; }

        /// <summary>
        /// Begins the rendering process for the window.
        /// </summary>
        /// <param name="config">The window configuration.</param>
        void BeginRendering(WindowConfiguration config);

        /// <summary>
        /// Ends the rendering process for the window.
        /// </summary>
        void EndRendering();

        /// <summary>
        /// Renders the editors using the provided draw action.
        /// </summary>
        /// <param name="drawEditorsAction">The action to draw editors.</param>
        void RenderEditors(Action drawEditorsAction);

        /// <summary>
        /// Occurs at the beginning of OnGUI.
        /// </summary>
        event Action BeginGUI;

        /// <summary>
        /// Occurs at the end of OnGUI.
        /// </summary>
        event Action EndGUI;
    }
}

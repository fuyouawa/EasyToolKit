using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Handles editor preview drawing
    /// </summary>
    internal class PreviewRenderingModule : EditorWindowModuleBase
    {
        private const int DEFAULT_EDITOR_PREVIEW_HEIGHT = 170;

        private float _defaultEditorPreviewHeight = DEFAULT_EDITOR_PREVIEW_HEIGHT;
        private bool _drawUnityEditorPreview;

        public float DefaultEditorPreviewHeight
        {
            get => _defaultEditorPreviewHeight;
            set => _defaultEditorPreviewHeight = value;
        }

        public bool DrawUnityEditorPreview
        {
            get => _drawUnityEditorPreview;
            set => _drawUnityEditorPreview = value;
        }

        public override void OnGUI()
        {
            // Preview rendering is handled within GUILayoutModule's DrawEditors method
            // This module provides the configuration and helper methods
        }

        /// <summary>
        /// Draws the editor preview for the target at the specified index
        /// </summary>
        /// <param name="index">The index of the target</param>
        /// <param name="height">The height of the preview area</param>
        public void DrawEditorPreview(int index, float height)
        {
            var editorModule = Window.GetModule<EditorManagementModule>();
            if (editorModule?.Editors == null) return;

            UnityEditor.Editor editor = editorModule.Editors[index];

            if (editor != null && editor.HasPreviewGUI())
            {
                Rect rect = EditorGUILayout.GetControlRect(false, height);
                editor.DrawPreview(rect);
            }
        }

        /// <summary>
        /// Checks if the target at the specified index has a preview available
        /// </summary>
        /// <param name="index">The index of the target</param>
        /// <returns>True if preview is available, false otherwise</returns>
        public bool HasPreviewGUI(int index)
        {
            var editorModule = Window.GetModule<EditorManagementModule>();
            if (editorModule?.Editors == null) return false;

            UnityEditor.Editor editor = editorModule.Editors[index];
            return editor != null && editor.HasPreviewGUI();
        }

        /// <summary>
        /// Gets the default preview height
        /// </summary>
        /// <returns>The default preview height in pixels</returns>
        public float GetDefaultPreviewHeight()
        {
            return _defaultEditorPreviewHeight;
        }

        /// <summary>
        /// Sets whether Unity editor previews should be drawn
        /// </summary>
        /// <param name="draw">True to draw previews, false otherwise</param>
        public void SetDrawUnityEditorPreview(bool draw)
        {
            _drawUnityEditorPreview = draw;
        }

        /// <summary>
        /// Sets the default preview height
        /// </summary>
        /// <param name="height">The height in pixels</param>
        public void SetDefaultPreviewHeight(float height)
        {
            _defaultEditorPreviewHeight = height;
        }

        public override void Dispose()
        {
            // No specific cleanup needed for preview rendering
        }
    }
}

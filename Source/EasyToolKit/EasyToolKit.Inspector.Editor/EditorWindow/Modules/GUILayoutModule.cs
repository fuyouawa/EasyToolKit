using System;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Handles scrolling, layout, and drawing logic
    /// </summary>
    internal class GUILayoutModule : EditorWindowModuleBase
    {
        private const int DEFAULT_WRAPPED_AREA_MAX_HEIGHT = 1000;
        private const float DEFAULT_LABEL_WIDTH_RATIO = 0.33f;

        private GUIStyle _marginStyle;
        private Vector2 _scrollPosition;
        private Vector2 _contentSize;
        private bool _useScrollView = true;
        private Vector4 _windowPadding = new Vector4(4, 4, 4, 4);
        private float _labelWidth = DEFAULT_LABEL_WIDTH_RATIO;
        private int _wrappedAreaMaxHeight = DEFAULT_WRAPPED_AREA_MAX_HEIGHT;

        public bool UseScrollView
        {
            get => _useScrollView;
            set => _useScrollView = value;
        }

        public Vector4 WindowPadding
        {
            get => _windowPadding;
            set => _windowPadding = value;
        }

        public float DefaultLabelWidth
        {
            get => _labelWidth;
            set => _labelWidth = value;
        }

        public int WrappedAreaMaxHeight
        {
            get => _wrappedAreaMaxHeight;
            set => _wrappedAreaMaxHeight = value;
        }

        public Vector2 ContentSize
        {
            get => _contentSize;
            set => _contentSize = value;
        }

        public override void OnGUI()
        {
            BeginLayoutArea();

            var size = BeginVerticalLayout();
            UpdateContentSize(size);

            ConfigureGUILayout();
            DrawContent();

            EndVerticalLayout();

            if (_useScrollView)
            {
                EditorGUILayout.EndScrollView();
            }

            EndLayoutArea();
        }

        /// <summary>
        /// Begins the wrapped area if content expansion is prevented
        /// </summary>
        private void BeginLayoutArea()
        {
            var windowModule = Window.GetModule<WindowManagementModule>();
            bool measureArea = windowModule?.PreventContentFromExpanding ?? false;

            if (measureArea)
            {
                var maxHeight = windowModule?.WrappedAreaMaxHeight ?? _wrappedAreaMaxHeight;
                GUILayout.BeginArea(new Rect(0, 0, Window.position.width, maxHeight));

                // Update content size for window module
                if (windowModule != null)
                {
                    windowModule.ContentSize = _contentSize;
                }
            }
        }

        /// <summary>
        /// Ends the wrapped area if content expansion is prevented
        /// </summary>
        private void EndLayoutArea()
        {
            var windowModule = Window.GetModule<WindowManagementModule>();
            bool measureArea = windowModule?.PreventContentFromExpanding ?? false;

            if (measureArea)
            {
                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// Begins the scroll view and vertical layout
        /// </summary>
        private Vector2 BeginVerticalLayout()
        {
            if (_useScrollView)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            }

            Vector2 size;
            var windowModule = Window.GetModule<WindowManagementModule>();
            bool preventExpansion = windowModule?.PreventContentFromExpanding ?? false;

            if (preventExpansion)
            {
                size = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false)).size;
            }
            else
            {
                size = EditorGUILayout.BeginVertical().size;
            }

            return size;
        }

        /// <summary>
        /// Ends the vertical layout
        /// </summary>
        private void EndVerticalLayout()
        {
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Updates the content size if needed
        /// </summary>
        private void UpdateContentSize(Vector2 size)
        {
            if (_contentSize == Vector2.zero || Event.current.type == EventType.Repaint)
            {
                _contentSize = size;
            }
        }

        /// <summary>
        /// Configures GUI helper settings for layout
        /// </summary>
        private void ConfigureGUILayout()
        {
            EasyGUIHelper.PushHierarchyMode(false);

            // Initialize margin style
            _marginStyle = _marginStyle ?? new GUIStyle();

            if (Event.current.type == EventType.Layout)
            {
                _marginStyle.padding.left = (int)_windowPadding.x;
                _marginStyle.padding.right = (int)_windowPadding.y;
                _marginStyle.padding.top = (int)_windowPadding.z;
                _marginStyle.padding.bottom = (int)_windowPadding.w;
            }

            // Calculate label width
            float calculatedLabelWidth;
            if (_labelWidth < 1)
            {
                calculatedLabelWidth = _contentSize.x * _labelWidth;
            }
            else
            {
                calculatedLabelWidth = _labelWidth;
            }

            EasyGUIHelper.PushLabelWidth(calculatedLabelWidth);
        }

        /// <summary>
        /// Draws the main content area
        /// </summary>
        private void DrawContent()
        {
            Window.OnBeginDrawEditors();
            GUILayout.BeginVertical(_marginStyle);

            DrawEditors();

            GUILayout.EndVertical();
            Window.OnEndDrawEditors();

            EasyGUIHelper.PopLabelWidth();
            EasyGUIHelper.PopHierarchyMode();
        }

        /// <summary>
        /// Draws all editors
        /// </summary>
        private void DrawEditors()
        {
            var editorModule = Window.GetModule<EditorManagementModule>();
            if (editorModule == null) return;

            for (int i = 0; i < editorModule.CurrentTargets.Count; i++)
            {
                // Draw the editor
                editorModule.DrawEditor(i);

                // Draw preview if enabled
                if (Window.DrawUnityEditorPreview)
                {
                    var previewModule = Window.GetModule<PreviewRenderingModule>();
                    previewModule?.DrawEditorPreview(i, Window.DefaultEditorPreviewHeight);
                }
            }
        }

        public override void Dispose()
        {
            _marginStyle = null;
        }
    }
}

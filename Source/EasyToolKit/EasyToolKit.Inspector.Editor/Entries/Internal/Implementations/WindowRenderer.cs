using System;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Handles the rendering of an EasyEditorWindow's GUI.
    /// </summary>
    internal sealed class WindowRenderer : IWindowRenderer
    {
        private const int DrawWarmupCount = 10;

        private readonly EditorWindow _window;
        private GUIStyle _marginStyle;
        private Vector2 _scrollPosition;
        private Vector2 _contentSize;
        private int _drawCountWarmup;

        /// <inheritdoc/>
        public Vector2 ContentSize => _contentSize;

        /// <inheritdoc/>
        public Vector2 ScrollPosition => _scrollPosition;

        /// <inheritdoc/>
        public event Action BeginGUI;

        /// <inheritdoc/>
        public event Action EndGUI;

        public WindowRenderer(EditorWindow window)
        {
            _window = window;
        }

        /// <inheritdoc/>
        public void BeginRendering(WindowConfiguration config)
        {
            bool measureArea = config.PreventContentFromExpanding;
            if (measureArea)
            {
                GUILayout.BeginArea(new Rect(0, 0, _window.position.width, config.WrappedAreaMaxHeight));
            }

            BeginGUI?.Invoke();

            _marginStyle = _marginStyle ?? new GUIStyle() { padding = new RectOffset() };

            if (Event.current.type == EventType.Layout)
            {
                _marginStyle.padding.left = (int)config.WindowPadding.x;
                _marginStyle.padding.right = (int)config.WindowPadding.y;
                _marginStyle.padding.top = (int)config.WindowPadding.z;
                _marginStyle.padding.bottom = (int)config.WindowPadding.w;
            }
        }

        /// <inheritdoc/>
        public void RenderEditors(Action drawEditorsAction)
        {
            var config = GetCurrentConfig();
            bool useScrollView = config.UseScrollView;

            if (useScrollView)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            }

            DrawEditorContent(config, drawEditorsAction);

            if (useScrollView)
            {
                EditorGUILayout.EndScrollView();
            }
        }

        /// <inheritdoc/>
        public void EndRendering()
        {
            var config = GetCurrentConfig();
            bool measureArea = config.PreventContentFromExpanding;

            EndGUI?.Invoke();

            HandleWarmupRepaints();

            var currentTargets = (_window as EasyEditorWindow)?.CurrentDrawingTargets;
            if (Event.current.isMouse || Event.current.type == EventType.Used ||
                currentTargets == null || currentTargets.Count == 0 ||
                EasyGUIHelper.CurrentWindowHasFocus)
            {
                _window.Repaint();
            }

            _window.RepaintIfRequested();

            if (measureArea)
            {
                GUILayout.EndArea();
            }
        }

        private void DrawEditorContent(WindowConfiguration config, Action drawEditorsAction)
        {
            Vector2 size;
            if (config.PreventContentFromExpanding)
            {
                size = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false)).size;
            }
            else
            {
                size = EditorGUILayout.BeginVertical().size;
            }

            if (_contentSize == Vector2.zero || Event.current.type == EventType.Repaint)
            {
                _contentSize = size;
            }

            EasyGUIHelper.PushHierarchyMode(false);
            float calculatedLabelWidth = CalculateLabelWidth(config, _contentSize.x);
            EasyGUIHelper.PushLabelWidth(calculatedLabelWidth);

            (_window as EasyEditorWindow)?.OnBeginDrawEditors();
            GUILayout.BeginVertical(_marginStyle);

            drawEditorsAction?.Invoke();

            GUILayout.EndVertical();
            (_window as EasyEditorWindow)?.OnEndDrawEditors();
            EasyGUIHelper.PopLabelWidth();
            EasyGUIHelper.PopHierarchyMode();

            EditorGUILayout.EndVertical();
        }

        private float CalculateLabelWidth(WindowConfiguration config, float contentWidth)
        {
            if (config.LabelWidth < 1)
            {
                return contentWidth * config.LabelWidth;
            }
            else
            {
                return config.LabelWidth;
            }
        }

        private void HandleWarmupRepaints()
        {
            if (_drawCountWarmup < DrawWarmupCount)
            {
                _window.Repaint();

                if (Event.current.type == EventType.Repaint)
                {
                    _drawCountWarmup++;
                }
            }
        }

        private WindowConfiguration GetCurrentConfig()
        {
            return (_window as EasyEditorWindow)?.Configuration ?? new WindowConfiguration();
        }
    }
}

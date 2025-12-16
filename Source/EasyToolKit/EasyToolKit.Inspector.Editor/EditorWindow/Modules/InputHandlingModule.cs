using System;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Manages mouse/keyboard interactions and focus control
    /// </summary>
    internal class InputHandlingModule : EditorWindowModuleBase
    {
        private const int COORDINATE_VALIDATION_FRAMES = 2;
        private const int DRAW_WARMUP_COUNT = 10;

        private int _drawCountWarmup;
        private int _mouseDownControlId;
        private EditorWindow _mouseDownWindow;
        private int _mouseDownKeyboardControl;

        public bool WantsMouseMove { get; set; } = true;

        public override void Initialize(EasyEditorWindow window)
        {
            base.Initialize(window);
            WantsMouseMove = true;
        }

        public override void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            Window.wantsMouseMove = WantsMouseMove;
        }

        public override void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        public override void OnGUI()
        {
            HandleInputEvents();
            HandleRepaintLogic();
            HandleWarmupRepaints();
        }

        /// <summary>
        /// Handles mouse and keyboard input events
        /// </summary>
        private void HandleInputEvents()
        {
            var previousEventType = Event.current.type;

            // Track mouse down events
            if (Event.current.type == EventType.MouseDown)
            {
                _mouseDownControlId = GUIUtility.hotControl;
                _mouseDownKeyboardControl = GUIUtility.keyboardControl;
                _mouseDownWindow = EditorWindow.focusedWindow;
            }

            // Handle mouse up events to remove focus from text fields when clicking on empty areas
            if (Event.current.type == EventType.MouseUp &&
                GUIUtility.hotControl == _mouseDownControlId &&
                EditorWindow.focusedWindow == _mouseDownWindow &&
                GUIUtility.keyboardControl == _mouseDownKeyboardControl)
            {
                RemoveFocusFromEmptyAreaClick();
            }

            // Reset control ID if event type changed
            if (Event.current.type != previousEventType)
            {
                _mouseDownControlId = -2;
            }
        }

        /// <summary>
        /// Removes focus from controls when clicking on empty areas
        /// </summary>
        private void RemoveFocusFromEmptyAreaClick()
        {
            EasyGUIHelper.RemoveFocusControl();
            GUI.FocusControl(null);
        }

        /// <summary>
        /// Handles repaint logic based on various conditions
        /// </summary>
        private void HandleRepaintLogic()
        {
            var editorModule = Window.GetModule<EditorManagementModule>();

            // TODO: Find out why the window doesn't repaint properly when this condition is false.
            // Try navigating a menu tree with the keyboard filled with menu items that have nothing to inspect.
            // It only updates when you start moving the mouse.
            bool shouldRepaint = Event.current.isMouse ||
                               Event.current.type == EventType.Used ||
                               editorModule?.CurrentTargets == null ||
                               editorModule?.CurrentTargets.Count == 0 ||
                               EasyGUIHelper.CurrentWindowHasFocus;

            if (shouldRepaint)
            {
                Window.Repaint();
            }
        }

        /// <summary>
        /// Handles warmup repaints for proper initialization
        /// </summary>
        private void HandleWarmupRepaints()
        {
            if (_drawCountWarmup < DRAW_WARMUP_COUNT)
            {
                Window.Repaint();

                if (Event.current.type == EventType.Repaint)
                {
                    _drawCountWarmup++;
                }
            }
        }

        /// <summary>
        /// Called when Unity selection changes
        /// </summary>
        private void OnSelectionChanged()
        {
            Window.Repaint();
        }

        public override void Dispose()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}

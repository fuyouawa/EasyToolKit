using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Handles window creation, positioning, and popup functionality
    /// </summary>
    internal class WindowManagementModule : EditorWindowModuleBase
    {
        private const int DEFAULT_INSPECT_OBJECT_WINDOW_COUNT = 3;
        private const int AUTOMATIC_HEIGHT_MAX_HEIGHT = 600;
        private const int AUTOMATIC_HEIGHT_SCREEN_MARGIN = 40;
        private const int AUTOMATIC_HEIGHT_FRAME_COUNT = 10;

        private static readonly object LockObject = new object();
        private static int inspectObjectWindowCounter;

        public bool PreventContentFromExpanding { get; set; }
        public int WrappedAreaMaxHeight { get; set; }
        public Vector2 ContentSize { get; set; }

        public override void OnGUI()
        {
            // Window-specific GUI logic handled here if needed
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, float windowWidth)
        {
            return InspectObjectInDropDown(obj, btnRect, new Vector2(windowWidth, 0));
        }

        /// <summary>
        /// Pops up an editor window for the given object in a drop-down window with automatic height adjustment
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, Vector2 windowSize)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);

            if (windowSize.x <= 1) windowSize.x = btnRect.width;
            if (windowSize.x <= 1) windowSize.x = 400;

            // Having floating point values can cause Unity's editor window to be transparent
            btnRect.x = (int)btnRect.x;
            btnRect.width = (int)btnRect.width;
            btnRect.height = (int)btnRect.height;
            btnRect.y = (int)btnRect.y;
            windowSize.x = (int)windowSize.x;
            windowSize.y = (int)windowSize.y;

            try
            {
                // Also repaint parent window when the drop down repaints
                var currentWindow = EasyGUIHelper.CurrentWindow;
                if (currentWindow != null)
                {
                    window.GuiBegun += () => currentWindow.Repaint();
                }
            }
            catch
            {
                // Ignore exceptions
            }

            // Draw lighter background for non-pro skin
            if (!EditorGUIUtility.isProSkin)
            {
                window.GuiBegun += () => EasyEditorGUI.DrawSolidRect(
                    new Rect(0, 0, window.position.width, window.position.height),
                    EasyGUIStyles.MenuBackgroundColor);
            }

            // Draw borders
            window.GuiEnded += () => EasyEditorGUI.DrawBorders(
                new Rect(0, 0, window.position.width, window.position.height), 1);

            window.DefaultLabelWidth = 0.33f;
            window.DrawUnityEditorPreview = true;
            btnRect.position = GUIUtility.GUIToScreenPoint(btnRect.position);

            if ((int)windowSize.y == 0)
            {
                window.ShowAsDropDown(btnRect, new Vector2(windowSize.x, 10));
                SetupAutomaticHeightAdjustment(window, AUTOMATIC_HEIGHT_MAX_HEIGHT);
            }
            else
            {
                window.ShowAsDropDown(btnRect, windowSize);
            }

            return window;
        }

        /// <summary>
        /// Sets up automatic height adjustment for dropdown windows
        /// </summary>
        private static void SetupAutomaticHeightAdjustment(EasyEditorWindow window, int maxHeight)
        {
            var module = window.GetModule<WindowManagementModule>();
            if (module == null) return;

            module.PreventContentFromExpanding = true;
            module.WrappedAreaMaxHeight = maxHeight;

            var screenHeight = Screen.currentResolution.height - AUTOMATIC_HEIGHT_SCREEN_MARGIN;
            var originalPosition = window.position;
            originalPosition.x = (int)originalPosition.x;
            originalPosition.y = (int)originalPosition.y;
            originalPosition.width = (int)originalPosition.width;
            originalPosition.height = (int)originalPosition.height;
            var currentPosition = originalPosition;
            var windowReference = window;
            var getGoodOriginalPointer = 0;
            var tempFrameCount = 0;

            EditorApplication.CallbackFunction callback = null;
            callback = () =>
            {
                EditorApplication.update -= callback;
                EditorApplication.update -= callback;

                if (windowReference == null) return;

                if (tempFrameCount++ < AUTOMATIC_HEIGHT_FRAME_COUNT)
                {
                    windowReference.Repaint();
                }

                // In the first frame the x and y coordinates are zero, so we must wait a frame
                if (getGoodOriginalPointer <= 1 && originalPosition.y < 1)
                {
                    getGoodOriginalPointer++;
                    originalPosition = windowReference.position;
                }
                else
                {
                    var currentContentHeight = (int)module.ContentSize.y;
                    if (currentContentHeight != currentPosition.height)
                    {
                        tempFrameCount = 0;
                        currentPosition = originalPosition;
                        currentPosition.height = (int)Math.Min(currentContentHeight, maxHeight);
                        windowReference.minSize = new Vector2(windowReference.minSize.x, currentPosition.height);
                        windowReference.maxSize = new Vector2(windowReference.maxSize.x, currentPosition.height);
                        if (currentPosition.yMax >= screenHeight)
                        {
                            var delta = currentPosition.yMax - screenHeight;
                            currentPosition.y -= delta;
                        }
                        windowReference.position = currentPosition;
                    }
                }
                EditorApplication.update += callback;
            };
            EditorApplication.update += callback;
        }

        /// <summary>
        /// Additional InspectObjectInDropDown overloads
        /// </summary>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, 350);
        }

        public static EasyEditorWindow InspectObjectInDropDown(object obj, float windowWidth)
        {
            var position = Event.current.mousePosition;
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position, float windowWidth)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        public static EasyEditorWindow InspectObjectInDropDown(object obj, float width, float height)
        {
            var r = new Rect(Event.current.mousePosition, Vector2.one);
            return InspectObjectInDropDown(obj, r, new Vector2(width, height));
        }

        public static EasyEditorWindow InspectObjectInDropDown(object obj)
        {
            return InspectObjectInDropDown(obj, Event.current.mousePosition);
        }

        /// <summary>
        /// Pops up a regular editor window for the given object
        /// </summary>
        public static EasyEditorWindow InspectObject(object obj)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);
            window.Show();

            lock (LockObject)
            {
                var offset = new Vector2(30, 30) * ((inspectObjectWindowCounter++ % 6) - 3);
                window.position = EasyGUIHelper.GetEditorWindowRect()
                    .AlignCenter(400, 300)
                    .AddPosition(offset);
            }

            return window;
        }

        /// <summary>
        /// Creates an editor window instance for the specified object, without opening the window
        /// </summary>
        public static EasyEditorWindow CreateEasyEditorWindowInstanceForObject(object obj)
        {
            var window = ScriptableObject.CreateInstance<EasyEditorWindow>();

            // In Unity version 2017.3+ the new window doesn't receive focus on the first click
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;

            var unityObject = obj as UnityEngine.Object;
            if (unityObject != null)
            {
                // If it's a Unity object, the reference can survive a recompile
                window.SetSerializedTarget(unityObject);
            }
            else
            {
                // Otherwise, it can't survive a recompile
                window.SetTargetObject(obj);
            }

            SetWindowTitle(window, obj, unityObject);
            window.position = EasyGUIHelper.GetEditorWindowRect().AlignCenter(600, 600);

            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Sets the window title based on the object type
        /// </summary>
        private static void SetWindowTitle(EasyEditorWindow window, object obj, UnityEngine.Object unityObject)
        {
            if (unityObject is Component component)
            {
                window.titleContent = new GUIContent(component.gameObject.name);
            }
            else if (unityObject != null)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }
        }

        public override void Dispose()
        {
            // Cleanup if needed
        }
    }
}

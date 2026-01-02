using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides utility methods for creating and configuring EasyEditorWindow instances.
    /// </summary>
    public static class EasyEditorWindowUtility
    {
        private const int AUTOMATIC_HEIGHT_MAX_HEIGHT = 600;
        private const int AUTOMATIC_HEIGHT_SCREEN_MARGIN = 40;
        private const int AUTOMATIC_HEIGHT_FRAME_COUNT = 10;
        private const float DEFAULT_LABEL_WIDTH_RATIO = 0.33f;

        private static readonly object LockObject = new object();
        private static int s_inspectObjectWindowCounter;

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// This particular overload uses a few frames to calculate the height of the content before showing the window with a height that matches its content.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="btnRect">The button rectangle to position the dropdown from.</param>
        /// <param name="windowWidth">The width of the window.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, float windowWidth)
        {
            return InspectObjectInDropDown(obj, btnRect, new Vector2(windowWidth, 0));
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="btnRect">The button rectangle to position the dropdown from.</param>
        /// <param name="windowSize">The size of the window. If y is 0, height will be automatically calculated.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Rect btnRect, Vector2 windowSize)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);

            if (windowSize.x <= 1) windowSize.x = btnRect.width;
            if (windowSize.x <= 1) windowSize.x = 400;

            // Having floating point values, can cause Unity's editor window to be transparent.
            btnRect.x = (int)btnRect.x;
            btnRect.width = (int)btnRect.width;
            btnRect.height = (int)btnRect.height;
            btnRect.y = (int)btnRect.y;
            windowSize.x = (int)windowSize.x;
            windowSize.y = (int)windowSize.y;

            try
            {
                // Also repaint parent window, when the drop down repaints.
                var currentWindow = EasyGUIHelper.CurrentWindow;
                if (currentWindow != null)
                {
                    window.BeginGUI += () => currentWindow.Repaint();
                }
            }
            catch
            {
            }

            // Draw lighter bg.
            if (!EditorGUIUtility.isProSkin)
            {
                window.BeginGUI += () => EasyEditorGUI.DrawSolidRect(new Rect(0, 0, window.position.width, window.position.height), EasyGUIStyles.MenuBackgroundColor);
            }

            // Draw borders.
            window.EndGUI += () => EasyEditorGUI.DrawBorders(new Rect(0, 0, window.position.width, window.position.height), 1);
            window.DefaultLabelWidth = DEFAULT_LABEL_WIDTH_RATIO;
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
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="position">The position to show the dropdown at.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, 350);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="windowWidth">The width of the window.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float windowWidth)
        {
            var position = Event.current.mousePosition;
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="position">The position to show the dropdown at.</param>
        /// <param name="windowWidth">The width of the window.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, Vector2 position, float windowWidth)
        {
            var btnRect = new Rect(position.x, position.y, 1, 1);
            return InspectObjectInDropDown(obj, btnRect, windowWidth);
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj, float width, float height)
        {
            var r = new Rect(Event.current.mousePosition, Vector2.one);
            return InspectObjectInDropDown(obj, r, new Vector2(width, height));
        }

        /// <summary>
        /// <para>
        /// Pops up an editor window for the given object in a drop-down window which closes when it loses its focus.
        /// </para>
        /// <para>Protip: You can subscribe to OnClose if you want to know when that occurs.</para>
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObjectInDropDown(object obj)
        {
            return InspectObjectInDropDown(obj, Event.current.mousePosition);
        }

        /// <summary>
        /// Pops up an editor window for the given object.
        /// </summary>
        /// <param name="obj">The object to inspect.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObject(object obj)
        {
            var window = CreateEasyEditorWindowInstanceForObject(obj);
            window.Show();

            lock (LockObject)
            {
                var offset = new Vector2(30, 30) * ((s_inspectObjectWindowCounter++ % 6) - 3);
                window.position = EasyGUIHelper.GetEditorWindowRect()
                    .AlignCenter(400, 300)
                    .AddPosition(offset);
            }

            return window;
        }

        /// <summary>
        /// Inspects the object using an existing EasyEditorWindow.
        /// </summary>
        /// <param name="window">The window to use for inspection.</param>
        /// <param name="obj">The object to inspect.</param>
        /// <returns>The configured EasyEditorWindow instance.</returns>
        public static EasyEditorWindow InspectObject(EasyEditorWindow window, object obj)
        {
            var unityObject = obj as UnityEngine.Object;
            if (unityObject)
            {
                // If it's a Unity object, then it's likely the reference can survive a recompile.
                window.TargetObject = null;
                window.SerializedTarget = unityObject;
            }
            else
            {
                // Otherwise, it can't. In which case we don't want want to serialize it - hence targetObject and not serializedTarget.
                // If we did the user would be inspecting a different reference than provided.
                window.SerializedTarget = null;
                window.TargetObject = obj;
            }

            if (unityObject as Component)
            {
                window.titleContent = new GUIContent((unityObject as Component).gameObject.name);
            }
            else if (unityObject)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }

            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Creates an editor window instance for the specified object, without opening the window.
        /// </summary>
        /// <param name="obj">The object to create an editor window for.</param>
        /// <returns>The created EasyEditorWindow instance.</returns>
        public static EasyEditorWindow CreateEasyEditorWindowInstanceForObject(object obj)
        {
            var window = ScriptableObject.CreateInstance<EasyEditorWindow>();

            // In Unity version 2017.3+ the new window doesn't recive focus on the first click if something from another window has focus.
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;

            var unityObject = obj as UnityEngine.Object;
            if (unityObject)
            {
                // If it's a Unity object, then it's likely the reference can survive a recompile.
                window.SerializedTarget = unityObject;
            }
            else
            {
                // Otherwise, it can't. In which case we don't want want to serialize it - targetObject and not serializedTarget.
                // If we did the user would be inspecting a different reference than provided.
                window.TargetObject = obj;
            }

            if (unityObject as Component)
            {
                window.titleContent = new GUIContent((unityObject as Component).gameObject.name);
            }
            else if (unityObject)
            {
                window.titleContent = new GUIContent(unityObject.name);
            }
            else
            {
                window.titleContent = new GUIContent(obj.ToString());
            }

            window.position = EasyGUIHelper.GetEditorWindowRect().AlignCenter(600, 600);

            EditorUtility.SetDirty(window);
            return window;
        }

        /// <summary>
        /// Sets up automatic height adjustment for the dropdown window to match its content height.
        /// </summary>
        /// <param name="window">The window to adjust.</param>
        /// <param name="maxHeight">The maximum height for the window.</param>
        private static void SetupAutomaticHeightAdjustment(EasyEditorWindow window, int maxHeight)
        {
            window.PreventContentFromExpanding = true;
            window.WrappedAreaMaxHeight = maxHeight;

            // TODO: The AUTOMATIC_HEIGHT_SCREEN_MARGIN pixels right now represents the bottom task bar on Windows.
            // We need a good way of getting screen a "real estate" rect.
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

                if (windowReference == null)
                {
                    return;
                }

                if (tempFrameCount++ < AUTOMATIC_HEIGHT_FRAME_COUNT)
                {
                    windowReference.Repaint();
                }

                // In the first frame the x and y coordinates are zero, so we must wait a frame, unless it's not zero.
                if (getGoodOriginalPointer <= 1 && originalPosition.y < 1)
                {
                    getGoodOriginalPointer++;
                    originalPosition = windowReference.position;
                }
                else
                {
                    var currentContentHeight = (int)windowReference.ContentSize.y;
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
    }
}

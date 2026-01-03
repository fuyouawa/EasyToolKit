using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Manages focus state for editor windows.
    /// </summary>
    internal sealed class FocusManager : IFocusManager
    {
        private int _mouseDownControlId;
        private EditorWindow _mouseDownWindow;
        private int _mouseDownKeyboardControl;

        /// <inheritdoc/>
        public void ProcessFocusEvents()
        {
            var previousEventType = Event.current.type;

            if (Event.current.type == EventType.MouseDown)
            {
                _mouseDownControlId = GUIUtility.hotControl;
                _mouseDownKeyboardControl = GUIUtility.keyboardControl;
                _mouseDownWindow = EditorWindow.focusedWindow;;
            }

            // This removes focus from text-fields when clicking on an empty area.
            if (Event.current.type != previousEventType)
            {
                _mouseDownControlId = -2;
            }

            if (Event.current.type == EventType.MouseUp &&
                GUIUtility.hotControl == _mouseDownControlId &&
                EditorWindow.focusedWindow == _mouseDownWindow &&
                GUIUtility.keyboardControl == _mouseDownKeyboardControl)
            {
                EasyGUIHelper.RemoveFocusControl();
                GUI.FocusControl(null);
            }
        }
    }
}

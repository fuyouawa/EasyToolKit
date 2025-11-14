using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EditorWindowExtensions
    {
        public static void CenterWindowWithRadio(this EditorWindow window, float widthRadio, float heightRadio)
        {
            var rect = EasyGUIHelper.GetEditorWindowRect();
            window.CenterWindow(rect.width * widthRadio, rect.height * heightRadio);
        }

        public static void CenterWindow(this EditorWindow window, float width, float height)
        {
            window.position = EasyGUIHelper.GetEditorWindowRect().AlignCenter(width, height);
        }
    }
}

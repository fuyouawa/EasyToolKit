using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    internal static class EditorHelper
    {
        private static GUIContent s_tempContent = new GUIContent();

        public static GUIContent TempContent(string text, string tooltip = null, Texture image = null)
        {
            s_tempContent.text = text;
            s_tempContent.tooltip = tooltip;
            s_tempContent.image = image;
            return s_tempContent;
        }
    }
}

using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using JetBrains.Annotations;
using System;
using EasyToolKit.TileWorldPro.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public static class TerrainDefinitionDrawerContext
    {
        [CanBeNull] public static Guid? SelectedGuid { get; set; }
        public static DrawMode SelectedDrawMode { get; set; }
    }

    public class TerrainDefinitionDrawer : EasyValueDrawer<TerrainDefinition>
    {
        private static readonly Color SelectedButtonColor = new Color(0, 0.7f, 1f, 1);

        protected override void DrawProperty(GUIContent label)
        {
            CallNextDrawer(label);

            EasyEditorGUI.Title("绘制工具");
            EasyEditorGUI.BeginBox();
            EditorGUILayout.BeginHorizontal();
            Button(DrawMode.Brush);
            EditorGUILayout.Space(3, false);
            Button(DrawMode.Eraser);
            EditorGUILayout.Space(3, false);
            Button(DrawMode.LineBrush);
            EditorGUILayout.Space(3, false);
            Button(DrawMode.RectangleBrush);

            EditorGUILayout.EndHorizontal();
            EasyEditorGUI.EndBox();
        }

        private bool IsSelected(DrawMode drawMode)
        {
            var guid = TerrainDefinitionDrawerContext.SelectedGuid;
            if (guid == null || guid != ValueEntry.SmartValue.Guid)
                return false;

            return TerrainDefinitionDrawerContext.SelectedDrawMode == drawMode;
        }

        private bool Button(DrawMode drawMode)
        {
            var isSelected = IsSelected(drawMode);
            if (isSelected)
            {
                EasyGUIHelper.PushColor(SelectedButtonColor);
            }
            var btnRect = GUILayoutUtility.GetRect(30, 30, GUILayout.ExpandWidth(false));
            var clicked = GUI.Button(btnRect, GUIContent.none);

            if (isSelected)
            {
                EasyGUIHelper.PopColor();
            }

            if (Event.current.type == EventType.Repaint)
            {
                var icon = TileWorldIcons.Instance.GetDrawModeIcon(drawMode);
                GUI.DrawTexture(btnRect.AlignCenter(25, 25), icon);
            }

            if (clicked)
            {
                var guid = TerrainDefinitionDrawerContext.SelectedGuid;
                if (guid != null &&
                    guid == ValueEntry.SmartValue.Guid &&
                    TerrainDefinitionDrawerContext.SelectedDrawMode == drawMode)
                {
                    TerrainDefinitionDrawerContext.SelectedGuid = null;
                }
                else
                {
                    TerrainDefinitionDrawerContext.SelectedGuid = ValueEntry.SmartValue.Guid;
                }

                TerrainDefinitionDrawerContext.SelectedDrawMode = drawMode;
            }

            return clicked;
        }
    }
}

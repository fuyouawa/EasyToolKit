using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class MetroFoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<MetroFoldoutGroupAttribute>
    {
        private static GUIStyle s_foldoutStyle;

        public static GUIStyle FoldoutStyle
        {
            get
            {
                if (s_foldoutStyle == null)
                {
                    s_foldoutStyle = new GUIStyle(EasyGUIStyles.Foldout)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_foldoutStyle.margin.top += 4;
                }
                return s_foldoutStyle;
            }
        }

         private static GUIStyle s_rightLabelStyle;

         public static GUIStyle RightLabelStyle
         {
             get
             {
                 if (s_rightLabelStyle == null)
                 {
                     s_rightLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                     {
                         alignment = TextAnchor.MiddleLeft,
                     };
                     s_rightLabelStyle.fontSize += 2;
                 }
                 return s_rightLabelStyle;
             }
         }

        private ICodeValueResolver<string> _labelResolver;
        [CanBeNull] private ICodeValueResolver<string> _tooltipResolver;
        [CanBeNull] private ICodeValueResolver<string> _rightLabelResolver;
        [CanBeNull] private ICodeValueResolver<Color> _sideLineColorGetterResolver;
        [CanBeNull] private ICodeValueResolver<Color> _rightLabelColorGetterResolver;
        [CanBeNull] private ICodeValueResolver<Texture> _iconTextureGetterResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();

            _labelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);

            if (Attribute.Tooltip.IsNotNullOrWhiteSpace())
            {
                _tooltipResolver = CodeValueResolver.Create<string>(Attribute.Tooltip, targetType, true);
            }

            if (Attribute.RightLabel.IsNotNullOrWhiteSpace())
            {
                _rightLabelResolver = CodeValueResolver.Create<string>(Attribute.RightLabel, targetType, true);
            }

            if (Attribute.RightLabelColorGetter.IsNotNullOrWhiteSpace())
            {
                _rightLabelColorGetterResolver = CodeValueResolver.Create<Color>(Attribute.RightLabelColorGetter, targetType);
            }

            if (Attribute.IconTextureGetter.IsNotNullOrWhiteSpace())
            {
                _iconTextureGetterResolver = CodeValueResolver.Create<Texture>(Attribute.IconTextureGetter, targetType);
            }

            if (Attribute.SideLineColorGetter.IsNotNullOrWhiteSpace())
            {
                _sideLineColorGetterResolver = CodeValueResolver.Create<Color>(Attribute.SideLineColorGetter, targetType);
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_tooltipResolver != null && _tooltipResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_rightLabelResolver != null && _rightLabelResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_rightLabelColorGetterResolver != null && _rightLabelColorGetterResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_iconTextureGetterResolver != null && _iconTextureGetterResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_sideLineColorGetterResolver != null && _sideLineColorGetterResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label, ref bool foldout)
        {
            var resolveTarget = this.GetTargetForResolver();

            EasyEditorGUI.BeginBox();

            GUILayout.Space(-3);
            EditorGUILayout.BeginHorizontal("Button", GUILayout.ExpandWidth(true), GUILayout.Height(30));

            Color sideLineColor = Color.green;
            if (_sideLineColorGetterResolver != null)
            {
                sideLineColor = _sideLineColorGetterResolver.Resolve(resolveTarget);
            }

            EasyGUIHelper.PushColor(sideLineColor);
            GUILayout.Box(GUIContent.none, EasyGUIStyles.WhiteBoxStyle, GUILayout.Width(3), GUILayout.Height(30));
            EasyGUIHelper.PopColor();

            if (_iconTextureGetterResolver != null)
            {
                var iconTexture = _iconTextureGetterResolver.Resolve(resolveTarget);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelResolver.Resolve(resolveTarget);
            var tooltipText = _tooltipResolver?.Resolve(resolveTarget);

            var foldoutRect = EditorGUILayout.GetControlRect(true, 30, FoldoutStyle);
            Property.State.Expanded = EasyEditorGUI.Foldout(
                foldoutRect,
                Property.State.Expanded,
                EditorHelper.TempContent(labelText, tooltipText),
                FoldoutStyle);

            if (_rightLabelResolver != null)
            {
                var rightLabel = EditorHelper.TempContent(_rightLabelResolver.Resolve(resolveTarget));
                var rightLabelColor = Color.white;
                if (_rightLabelColorGetterResolver != null)
                {
                    rightLabelColor = _rightLabelColorGetterResolver.Resolve(resolveTarget);
                }

                var rightLabelSize = RightLabelStyle.CalcSize(rightLabel);

                EasyGUIHelper.PushColor(rightLabelColor);
                GUI.Label(foldoutRect.AlignRight(rightLabelSize.x).SubY(2f), rightLabel, RightLabelStyle);
                EasyGUIHelper.PopColor();
            }

            EditorGUILayout.EndHorizontal();

            foldout = Property.State.Expanded;
        }

        protected override void EndDrawGroup()
        {
            EasyEditorGUI.EndBox();
        }
    }
}

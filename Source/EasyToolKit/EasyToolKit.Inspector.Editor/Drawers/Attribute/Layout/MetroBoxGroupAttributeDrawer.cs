using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class MetroBoxGroupAttributeDrawer : EasyGroupAttributeDrawer<MetroBoxGroupAttribute>
    {
        private static GUIStyle s_boxHeaderLabelStyle;
        public static GUIStyle BoxHeaderLabelStyle
        {
            get
            {
                if (s_boxHeaderLabelStyle == null)
                {
                    s_boxHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_boxHeaderLabelStyle.margin.top += 4;
                }
                return s_boxHeaderLabelStyle;
            }
        }

        public static readonly GUIStyle BoxContainerStyle = new GUIStyle("TextArea")
        {
        };

        public static readonly Color HeaderBoxBackgroundColor = EasyGUIStyles.HeaderBoxBackgroundColor * 0.9f;

        private IExpressionEvaluator<string> _labelEvaluator;
        private IExpressionEvaluator<Texture> _iconTextureGetterEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

            _labelEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.Label, targetType)
                .WithExpressionFlag()
                .Build();
            _iconTextureGetterEvaluator = ExpressionEvaluatorFactory
                .Evaluate<Texture>(Attribute.IconTextureGetter, targetType)
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty() && _iconTextureGetterEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            Texture iconTexture = null;
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty())
            {
                iconTexture = _iconTextureGetterEvaluator.Evaluate(resolveTarget);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelEvaluator.Evaluate(resolveTarget);

            BeginDraw(EditorHelper.TempContent(labelText), iconTexture);
        }

        protected override void EndDrawGroup()
        {
            EndDraw();
        }

        public static void BeginDraw(GUIContent label, Texture iconTexture)
        {
            EasyEditorGUI.BeginIndentedVertical(BoxContainerStyle);

            GUILayout.Space(-3);
            var headerBgRect = EditorGUILayout.BeginHorizontal(EasyGUIStyles.BoxHeaderStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));

            if (Event.current.type == EventType.Repaint)
            {
                headerBgRect.x -= 3;
                headerBgRect.width += 6;
                EasyGUIHelper.PushColor(HeaderBoxBackgroundColor);
                GUI.DrawTexture(headerBgRect, Texture2D.whiteTexture);
                EasyGUIHelper.PopColor();
                EasyEditorGUI.DrawBorders(headerBgRect, 0, 0, 0, 1, EasyGUIStyles.BorderColor);
            }

            if (iconTexture != null)
            {
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            GUILayout.Label(label, BoxHeaderLabelStyle, GUILayout.Height(30));

            EditorGUILayout.EndHorizontal();
        }

        public static void EndDraw()
        {
            EasyEditorGUI.EndIndentedVertical();
        }
    }
}

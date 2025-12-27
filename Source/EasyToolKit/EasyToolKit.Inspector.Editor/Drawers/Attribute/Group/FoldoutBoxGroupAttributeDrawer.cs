using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class FoldoutBoxGroupAttributeDrawer : EasyGroupAttributeDrawer<FoldoutBoxGroupAttribute>
    {
        private ICodeValueResolver<string> _labelResolver;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _labelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            EasyEditorGUI.BeginBox();
            EasyEditorGUI.BeginBoxHeader();
            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var labelText = _labelResolver.Resolve(resolveTarget);
            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, EditorHelper.TempContent(labelText));
            EasyEditorGUI.EndBoxHeader();
        }

        protected override void EndDrawGroup()
        {
            EasyEditorGUI.EndBox();
        }
    }
}

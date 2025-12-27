using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class FoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<FoldoutGroupAttribute>
    {
        private ICodeValueResolver<string> _labelResolver;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element.LogicalChildren[0], Attribute);
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
            var target = ElementUtility.GetOwnerWithAttribute(Element.LogicalChildren[0], Attribute);
            var labelText = _labelResolver.Resolve(target);
            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, EditorHelper.TempContent(labelText));

            EditorGUI.indentLevel++;
        }

        protected override void EndDrawGroup()
        {
            EditorGUI.indentLevel--;
        }
    }
}

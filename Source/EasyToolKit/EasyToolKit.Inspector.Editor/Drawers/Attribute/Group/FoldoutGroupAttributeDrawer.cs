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
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

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
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);
            var labelText = _labelResolver.Resolve(resolveTarget);
            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, EditorHelper.TempContent(labelText));

            EditorGUI.indentLevel++;
        }

        protected override void EndDrawGroup()
        {
            EditorGUI.indentLevel--;
        }
    }
}

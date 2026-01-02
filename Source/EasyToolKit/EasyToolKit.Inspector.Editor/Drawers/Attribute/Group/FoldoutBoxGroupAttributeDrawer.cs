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
        private IExpressionEvaluator<string> _labelEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

            _labelEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.Label, targetType)
                .WithExpressionFlag()
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelEvaluator.TryGetError(out var error))
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
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);
            var labelText = _labelEvaluator.Evaluate(resolveTarget);
            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, EditorHelper.TempContent(labelText));
            EasyEditorGUI.EndBoxHeader();
        }

        protected override void EndDrawGroup()
        {
            EasyEditorGUI.EndBox();
        }
    }
}

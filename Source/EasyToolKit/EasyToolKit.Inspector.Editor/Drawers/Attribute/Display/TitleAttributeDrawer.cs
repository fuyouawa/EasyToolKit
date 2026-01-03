using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class TitleAttributeDrawer : EasyAttributeDrawer<TitleAttribute>
    {
        private IExpressionEvaluator<string> _titleEvaluator;
        private IExpressionEvaluator<string> _subtitleEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _titleEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.Title, targetType)
                .WithExpressionFlag()
                .Build();
            _subtitleEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.Subtitle, targetType)
                .WithExpressionFlag()
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_titleEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }
            if (_subtitleEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var titleText = _titleEvaluator.Evaluate(resolveTarget);
            var subtitleText = _subtitleEvaluator.Evaluate(resolveTarget);
            EasyEditorGUI.Title(titleText, subtitleText, Attribute.TextAlignment, Attribute.HorizontalLine, Attribute.BoldTitle);

            CallNextDrawer(label);
        }
    }
}
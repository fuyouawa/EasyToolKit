using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class LabelTextAttributeDrawer : EasyAttributeDrawer<LabelTextAttribute>
    {
        private IExpressionEvaluator<string> _labelEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

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

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            label.text = _labelEvaluator.Evaluate(resolveTarget);
            CallNextDrawer(label);
        }
    }
}

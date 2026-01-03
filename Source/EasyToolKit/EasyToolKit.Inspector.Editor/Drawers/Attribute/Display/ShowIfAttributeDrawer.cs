using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class ShowIfAttributeDrawer : EasyAttributeDrawer<ShowIfAttribute>
    {
        private IExpressionEvaluator _conditionEvaluator;
        private bool _show = false;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _conditionEvaluator = ExpressionEvaluatorFactory
                .Evaluate<object>(Attribute.Condition, targetType)
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_conditionEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
                var condition = _conditionEvaluator.Evaluate(resolveTarget);
                var value = Attribute.Value;
                _show = Equals(condition, value);
            }

            if (_show)
            {
                CallNextDrawer(label);
            }
        }
    }
}

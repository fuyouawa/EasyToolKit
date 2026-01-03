using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class HeaderAttributeDrawer : EasyAttributeDrawer<HeaderAttribute>
    {
        private IExpressionEvaluator<string> _headerEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _headerEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.header, targetType)
                .WithExpressionFlag()
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_headerEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var headerText = _headerEvaluator.Evaluate(resolveTarget);
            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
            CallNextDrawer(label);
        }
    }
}

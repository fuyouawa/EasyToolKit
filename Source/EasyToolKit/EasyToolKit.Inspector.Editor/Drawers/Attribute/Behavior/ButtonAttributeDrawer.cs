using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    //TODO support parameters
    public class ButtonAttributeDrawer : EasyMethodAttributeDrawer<ButtonAttribute>
    {
        [CanBeNull] private IExpressionEvaluator<string> _buttonLabelEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            if (Attribute.Label.IsNotNullOrEmpty())
            {
                _buttonLabelEvaluator = ExpressionEvaluatorFactory
                    .Evaluate<string>(Attribute.Label, targetType)
                    .WithExpressionFlag()
                    .Build();
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_buttonLabelEvaluator != null && _buttonLabelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var buttonLabel = _buttonLabelEvaluator != null
                ? _buttonLabelEvaluator.Evaluate(resolveTarget)
                : label.text;
            if (GUILayout.Button(buttonLabel))
            {
                foreach (var target in Element.LogicalParent.CastValue().ValueEntry.EnumerateWeakValues())
                {
                    if (target == null)
                        continue;
                    MethodInfo.Invoke(target, null);
                }
            }
        }
    }
}

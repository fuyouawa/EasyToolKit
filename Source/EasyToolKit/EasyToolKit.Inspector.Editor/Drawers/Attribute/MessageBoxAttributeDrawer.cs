using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class MessageBoxAttributeDrawer : EasyAttributeDrawer<MessageBoxAttribute>
    {
        [CanBeNull] private IExpressionEvaluator<bool> _visibleIfEvaluator;
        private IExpressionEvaluator<string> _messageEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            if (Attribute.VisibleIf != null)
            {
                _visibleIfEvaluator = ExpressionEvaluatorFactory
                    .Evaluate<bool>(Attribute.VisibleIf, targetType)
                    .Build();
            }

            _messageEvaluator = ExpressionEvaluatorFactory
                .Evaluate<string>(Attribute.Message, targetType)
                .WithExpressionFlag()
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_visibleIfEvaluator != null && _visibleIfEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_messageEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);

            if (_visibleIfEvaluator != null)
            {
                var visible = _visibleIfEvaluator.Evaluate(resolveTarget);
                if (!visible)
                {
                    CallNextDrawer(label);
                    return;
                }
            }

            var message = _messageEvaluator.Evaluate(resolveTarget);
            EasyEditorGUI.MessageBox(message, Attribute.MessageType switch
            {
                MessageBoxType.None => MessageType.None,
                MessageBoxType.Info => MessageType.Info,
                MessageBoxType.Warning => MessageType.Warning,
                MessageBoxType.Error => MessageType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(Attribute.MessageType), Attribute.MessageType, "Unknown message type"),
            });

            CallNextDrawer(label);
        }
    }
}
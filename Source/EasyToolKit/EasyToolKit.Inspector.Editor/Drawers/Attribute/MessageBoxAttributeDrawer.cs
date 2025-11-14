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
        [CanBeNull] private ICodeValueResolver<bool> _visibleIfResolver;
        private ICodeValueResolver<string> _messageResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();

            if (Attribute.VisibleIf != null)
            {
                _visibleIfResolver = CodeValueResolver.Create<bool>(Attribute.VisibleIf, targetType, false);
            }

            _messageResolver = CodeValueResolver.Create<string>(Attribute.Message, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_visibleIfResolver != null && _visibleIfResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_messageResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = this.GetTargetForResolver();

            if (_visibleIfResolver != null)
            {
                var visible = _visibleIfResolver.Resolve(resolveTarget);
                if (!visible)
                {
                    CallNextDrawer(label);
                    return;
                }
            }

            var message = _messageResolver.Resolve(resolveTarget);
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
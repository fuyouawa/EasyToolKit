using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class TitleAttributeDrawer : EasyAttributeDrawer<TitleAttribute>
    {
        private ICodeValueResolver<string> _titleResolver;
        private ICodeValueResolver<string> _subtitleResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();

            _titleResolver = CodeValueResolver.Create<string>(Attribute.Title, targetType, true);
            _subtitleResolver = CodeValueResolver.Create<string>(Attribute.Subtitle, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_titleResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }
            if (_subtitleResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = this.GetTargetForResolver();
            var titleText = _titleResolver.Resolve(resolveTarget);
            var subtitleText = _subtitleResolver.Resolve(resolveTarget);
            EasyEditorGUI.Title(titleText, subtitleText, Attribute.TextAlignment, Attribute.HorizontalLine, Attribute.BoldTitle);

            CallNextDrawer(label);
        }
    }
}
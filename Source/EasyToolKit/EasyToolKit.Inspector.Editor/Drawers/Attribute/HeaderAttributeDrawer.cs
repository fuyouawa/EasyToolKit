using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class HeaderAttributeDrawer : EasyAttributeDrawer<HeaderAttribute>
    {
        private ICodeValueResolver<string> _headerResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();

            _headerResolver = CodeValueResolver.Create<string>(Attribute.header, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_headerResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = this.GetTargetForResolver();
            var headerText = _headerResolver.Resolve(resolveTarget);
            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
            CallNextDrawer(label);
        }
    }
}

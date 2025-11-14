using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class ShowIfAttributeDrawer : EasyAttributeDrawer<ShowIfAttribute>
    {
        private ICodeValueResolver _conditionResolver;
        private bool _show = false;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();

            _conditionResolver = CodeValueResolver.CreateWeak(Attribute.Condition, targetType);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_conditionResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                var resolveTarget = this.GetTargetForResolver();
                var condition = _conditionResolver.ResolveWeak(resolveTarget);
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

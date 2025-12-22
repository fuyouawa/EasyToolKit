using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class HideIfAttributeDrawer : EasyAttributeDrawer<HideIfAttribute>
    {
        private ICodeValueResolver _conditionResolver;
        private bool _hide;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _conditionResolver = CodeValueResolver.CreateWeak(Attribute.Condition, targetType);
        }

        protected override void Draw(GUIContent label)
        {
            if (_conditionResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
                var condition = _conditionResolver.ResolveWeak(resolveTarget);
                var value = Attribute.Value;
                _hide = Equals(condition, value);
            }

            if (!_hide)
            {
                CallNextDrawer(label);
            }
        }
    }
}

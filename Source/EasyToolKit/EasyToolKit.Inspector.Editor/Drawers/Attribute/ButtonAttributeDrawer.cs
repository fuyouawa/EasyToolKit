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
        [CanBeNull] private ICodeValueResolver<string> _buttonLabelResolver;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            if (Attribute.Label.IsNotNullOrEmpty())
            {
                _buttonLabelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_buttonLabelResolver != null && _buttonLabelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var buttonLabel = _buttonLabelResolver != null
                ? _buttonLabelResolver.Resolve(resolveTarget)
                : label.text;
            if (GUILayout.Button(buttonLabel))
            {
                foreach (var target in Element.LogicalParent!.ValueEntry.EnumerateWeakValues())
                {
                    if (target == null)
                        continue;
                    MethodInfo.Invoke(target, null);
                }
            }
        }
    }
}

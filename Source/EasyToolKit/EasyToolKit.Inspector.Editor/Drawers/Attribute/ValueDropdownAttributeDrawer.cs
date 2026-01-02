using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    //TODO conflict handling
    public class ValueDropdownAttributeDrawer<T> : EasyAttributeDrawer<ValueDropdownAttribute, T>
    {
        private IExpressionEvaluator<object> _optionsGetterEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);
            _optionsGetterEvaluator = ExpressionEvaluatorFactory
                .Evaluate<object>(Attribute.OptionsGetter, targetType)
                .Build();
        }

        protected override void Draw(GUIContent label)
        {
            if (_optionsGetterEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var target = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var options = _optionsGetterEvaluator.Evaluate(target);
            var dropdownItems = new ValueDropdownList();
            if (options is IEnumerable<IValueDropdownItem> valueDropdownItems)
            {
                dropdownItems.AddRange(valueDropdownItems);
            }
            else if (options is IEnumerable<object> objectOptions)
            {
                foreach (var item in objectOptions)
                {
                    dropdownItems.Add(item);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"The return type of '{Attribute.OptionsGetter}' must be IEnumerable<IValueDropdownItem> or IEnumerable<object>");
            }

            if (Element.Definition.Roles.IsCollection())
            {
                CollectionDrawerStaticContext.NextElementDropdownListGetter = () => dropdownItems;
                CallNextDrawer(label);
                CollectionDrawerStaticContext.NextElementDropdownListGetter = null;
            }
            else
            {
                var value = ValueEntry.WeakSmartValue;
                var selectedIndex = dropdownItems.FindIndex(item => item.GetValue().Equals(value));

                EditorGUI.BeginChangeCheck();
                var newSelectedIndex = EasyEditorGUI.ValueDropdown(
                    label,
                    selectedIndex,
                    dropdownItems,
                    (index, val) => new GUIContent(val.GetText()));
                if (EditorGUI.EndChangeCheck())
                {
                    if (newSelectedIndex != selectedIndex && newSelectedIndex != -1)
                    {
                        value = dropdownItems[newSelectedIndex].GetValue();
                        ValueEntry.WeakSmartValue = value;
                    }
                }
            }
        }
    }
}

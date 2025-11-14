using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    //TODO conflict handling
    public class ValueDropdownAttributeDrawer : EasyAttributeDrawer<ValueDropdownAttribute>
    {
        private ICodeValueResolver<object> _optionsGetterResolver;

        protected override void Initialize()
        {
            var targetType = this.GetTargetTypeForResolver();
            _optionsGetterResolver = CodeValueResolver.Create<object>(Attribute.OptionsGetter, targetType);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_optionsGetterResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var target = this.GetTargetForResolver();
            var options = _optionsGetterResolver.Resolve(target);
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

            if (Property.ChildrenResolver is ICollectionResolver)
            {
                CollectionDrawerStaticContext.NextElementDropdownListGetter = () => dropdownItems;
                CallNextDrawer(label);
                CollectionDrawerStaticContext.NextElementDropdownListGetter = null;
            }
            else
            {
                var value = Property.ValueEntry.WeakSmartValue;
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
                        Property.ValueEntry.WeakSmartValue = value;
                    }
                }
            }
        }
    }
}

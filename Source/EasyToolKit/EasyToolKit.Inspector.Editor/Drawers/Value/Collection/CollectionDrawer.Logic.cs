using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.ThirdParty.OdinSerializer;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public partial class CollectionDrawer<T>
    {
        [CanBeNull] private Action<object, object> _onAddedItemCallback;
        [CanBeNull] private Action<object, object> _onRemovedItemCallback;

        [CanBeNull] private Func<object, object> _customCreateItemFunction;
        [CanBeNull] private Action<object, object> _customRemoveItemFunction;
        [CanBeNull] private Action<object, int> _customRemoveIndexFunction;

        private int? _insertAt;
        private int? _removeAt;
        private object[] _removeValues;

        private void InitializeLogic()
        {
            if (_listDrawerSettings == null) return;

            _onAddedItemCallback = SetupItemCallback(_listDrawerSettings.OnAddedItemCallback);
            _onRemovedItemCallback = SetupItemCallback(_listDrawerSettings.OnRemovedItemCallback);
            _customCreateItemFunction = SetupCustomCreateFunction(_listDrawerSettings.CustomCreateItemFunction);
            _customRemoveItemFunction = SetupCustomRemoveFunction(_listDrawerSettings.CustomRemoveItemFunction);
        }

        private Action<object, object> SetupItemCallback(string callbackName)
        {
            if (!callbackName.IsNotNullOrWhiteSpace()) return null;

            var methodInfo = FindItemCallbackMethod(callbackName);
            ValidateItemCallbackMethod(methodInfo, callbackName);

            var parameters = methodInfo.GetParameters();
            return parameters.Length switch
            {
                0 => (instance, value) => methodInfo.Invoke(instance, null),
                1 => CreateSingleParameterCallback(methodInfo),
                _ => throw new Exception($"The parameter count of '{callbackName}' in '{_listDrawerTargetType}' must be 0 or 1")
            };
        }

        private Action<object, object> CreateSingleParameterCallback(System.Reflection.MethodInfo methodInfo)
        {
            var parameterType = methodInfo.GetParameters()[0].ParameterType;
            if (!ValueEntry.ItemType.IsAssignableFrom(parameterType))
            {
                throw new Exception(
                    $"The parameter type of '{methodInfo.Name}' in '{_listDrawerTargetType}' must be '{ValueEntry.ItemType}'");
            }

            return (instance, value) => methodInfo.Invoke(instance, new object[] { value });
        }

        private Func<object, object> SetupCustomCreateFunction(string functionName)
        {
            if (!functionName.IsNotNullOrWhiteSpace()) return null;

            var methodInfo = _listDrawerTargetType.GetMethodEx(functionName, BindingFlagsHelper.All)
                             ?? throw new Exception($"Cannot find method '{functionName}' in '{_listDrawerTargetType}'");

            return instance => methodInfo.Invoke(instance, null);
        }

        private Action<object, object> SetupCustomRemoveFunction(string functionName)
        {
            if (!functionName.IsNotNullOrWhiteSpace()) return null;

            var methodInfo = _listDrawerTargetType.GetMethodEx(functionName, BindingFlagsHelper.All)
                             ?? throw new Exception($"Cannot find method '{functionName}' in '{_listDrawerTargetType}'");

            return (instance, value) => methodInfo.Invoke(instance, new object[] { value });
        }

        private System.Reflection.MethodInfo FindItemCallbackMethod(string callbackName)
        {
            return _listDrawerTargetType.GetMethods(BindingFlagsHelper.AllInstance)
                       .FirstOrDefault(m => m.Name == callbackName)
                   ?? throw new Exception($"Cannot find method '{callbackName}' in '{_listDrawerTargetType}'");
        }

        private void ValidateItemCallbackMethod(System.Reflection.MethodInfo methodInfo, string callbackName)
        {
            if (methodInfo.ReturnType != typeof(void))
            {
                throw new Exception($"Method '{callbackName}' in '{_listDrawerTargetType}' must return void");
            }
        }

        private void UpdateLogic()
        {
            if (_orderedCollectionAccessor != null)
            {
                if (_removeAt != null && Event.current.type == EventType.Repaint)
                {
                    try
                    {
                        DoRemoveItemAt(_removeAt.Value);
                    }
                    finally
                    {
                        _removeAt = null;
                    }

                    EasyGUIHelper.RequestRepaint();
                }
            }
            else if (_removeValues != null && Event.current.type == EventType.Repaint)
            {
                try
                {
                    DoRemoveItem(_removeValues);
                }
                finally
                {
                    _removeValues = null;
                }

                EasyGUIHelper.RequestRepaint();
            }
        }

        protected virtual void DoAddItem(Rect addButtonRect)
        {
            if (_elementDropdownListGetter != null)
            {
                var dropdownItems = _elementDropdownListGetter();
                EasyEditorGUI.ShowValueDropdownMenu(addButtonRect, null, dropdownItems, (item) =>
                {
                    var value = item.GetValue();
                    DoAddItem(value);
                }, (item) => new GUIContent(item.GetText()));
                return;
            }

            for (int i = 0; i < Element.SharedContext.Tree.Targets.Count; i++)
            {
                DoAddItem(GetValueToAdd(i));
            }
        }

        private void DoAddItem(object valueToAdd)
        {
            for (int i = 0; i < Element.SharedContext.Tree.Targets.Count; i++)
            {
                DoAddItem(i, valueToAdd);
            }
        }

        protected virtual void DoAddItem(int targetIndex, object valueToAdd)
        {
            var parent = Element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
            ValueEntry.EnqueueChange(() =>
            {
                ValueEntry.AddWeakItem(targetIndex, valueToAdd);
                _onAddedItemCallback?.Invoke(parent, valueToAdd);
            });
        }

        private void DoInsertItem(int index, object valueToAdd)
        {
            for (int i = 0; i < Element.SharedContext.Tree.Targets.Count; i++)
            {
                DoInsertItem(i, index, valueToAdd);
            }
        }

        private void DoInsertItem(int targetIndex, int index, object valueToAdd)
        {
            // Use the new ordered collection operation
            if (_orderedCollectionAccessor != null)
            {
                var parent = Element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
                ValueEntry.EnqueueChange(() =>
                {
                    _orderedCollectionAccessor.InsertWeakItemAt(targetIndex, index, valueToAdd);
                    _onAddedItemCallback?.Invoke(parent, valueToAdd);
                });
            }
            else
            {
                throw new InvalidOperationException(
                    $"The property '{Element.Path}' is not ordered collection, so you cannot insert elements at a specific index.");
            }
        }

        protected virtual void DoRemoveItemAt(int index)
        {
            for (int i = 0; i < Element.SharedContext.Tree.Targets.Count; i++)
            {
                DoRemoveItemAt(i, index);
            }
        }

        protected virtual void DoRemoveItemAt(int targetIndex, int index)
        {
            var parent = Element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
            if (_customRemoveIndexFunction != null)
            {
                _customRemoveIndexFunction.Invoke(parent, index);
            }
            else
            {
                Assert.IsTrue(_orderedCollectionAccessor != null);
                ValueEntry.EnqueueChange(() =>
                {
                    var valueToRemove = Element.LogicalChildren[index].ValueEntry.GetWeakValue(targetIndex);
                    _orderedCollectionAccessor.RemoveItemAt(targetIndex, index);
                    _onRemovedItemCallback?.Invoke(parent, valueToRemove);
                });
            }
        }

        private void DoRemoveItem(object[] values)
        {
            for (int i = 0; i < Element.SharedContext.Tree.Targets.Count; i++)
            {
                DoRemoveItem(i, values[i]);
            }
        }

        private void DoRemoveItem(int targetIndex, object valueToRemove)
        {
            var parent = Element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
            if (_customRemoveItemFunction != null)
            {
                _customRemoveItemFunction.Invoke(parent, valueToRemove);
            }
            else
            {
                ValueEntry.EnqueueChange(() =>
                {
                    ValueEntry.RemoveWeakItem(targetIndex, valueToRemove);
                    _onRemovedItemCallback?.Invoke(parent, valueToRemove);
                });
            }
        }

        protected virtual object GetValueToAdd(int targetIndex)
        {
            var parent = Element.LogicalParent.CastValue().ValueEntry.GetWeakValue(targetIndex);
            if (_customCreateItemFunction != null)
            {
                return _customCreateItemFunction.Invoke(parent);
            }

            if (ValueEntry.ItemType.IsInheritsFrom<UnityEngine.Object>())
            {
                return null;
            }

            return UnitySerializationUtility.CreateDefaultUnityInitializedObject(ValueEntry.ItemType);
        }
    }
}

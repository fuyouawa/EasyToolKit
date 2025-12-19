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
        [CanBeNull] private Action<object, object> _onAddedElementCallback;
        [CanBeNull] private Action<object, object> _onRemovedElementCallback;

        [CanBeNull] private Func<object, object> _customCreateElementFunction;
        [CanBeNull] private Action<object, object> _customRemoveElementFunction;
        [CanBeNull] private Action<object, int> _customRemoveIndexFunction;

        private int? _insertAt;
        private int? _removeAt;
        private object[] _removeValues;

        private void InitializeLogic()
        {
            if (_listDrawerSettings != null)
            {
                if (_listDrawerSettings.OnAddedElementCallback.IsNotNullOrEmpty())
                {
                    var onAddedElementMethod = _listDrawerTargetType.GetMethods(BindingFlagsHelper.AllInstance)
                                                   .FirstOrDefault(m => m.Name == _listDrawerSettings.OnAddedElementCallback)
                                               ?? throw new Exception($"Cannot find method '{_listDrawerSettings.OnAddedElementCallback}' in '{_listDrawerTargetType}'");

                    if (onAddedElementMethod.ReturnType != typeof(void))
                    {
                        throw new Exception(
                            $"Method '{_listDrawerSettings.OnAddedElementCallback}' in '{_listDrawerTargetType}' must return void");
                    }
                    var parameters = onAddedElementMethod.GetParameters();
                    if (parameters.Length == 0)
                    {
                        _onAddedElementCallback = (instance, value) =>
                        {
                            onAddedElementMethod.Invoke(instance, null);
                        };
                    }
                    else if (parameters.Length == 1)
                    {
                        if (!_collectionStructureResolver.ElementType.IsAssignableFrom(parameters[0].ParameterType))
                        {
                            throw new Exception(
                                $"The parameter type of '{_listDrawerSettings.OnAddedElementCallback}' in '{_listDrawerTargetType}' must be '{_collectionStructureResolver.ElementType}'");
                        }

                        _onAddedElementCallback = (instance, value) =>
                        {
                            onAddedElementMethod.Invoke(instance, new object[] { value });
                        };
                    }
                    else
                    {
                        throw new Exception($"The parameter count of '{_listDrawerSettings.OnAddedElementCallback}' in '{_listDrawerTargetType}' must be 0 or 1");
                    }
                }

                if (_listDrawerSettings.OnRemovedElementCallback.IsNotNullOrEmpty())
                {
                    var onRemovedElementMethod = _listDrawerTargetType.GetMethods(BindingFlagsHelper.AllInstance)
                                                   .FirstOrDefault(m => m.Name == _listDrawerSettings.OnRemovedElementCallback)
                                               ?? throw new Exception($"Cannot find method '{_listDrawerSettings.OnRemovedElementCallback}' in '{_listDrawerTargetType}'");

                    if (onRemovedElementMethod.ReturnType != typeof(void))
                    {
                        throw new Exception(
                            $"Method '{_listDrawerSettings.OnRemovedElementCallback}' in '{_listDrawerTargetType}' must return void");
                    }
                    var parameters = onRemovedElementMethod.GetParameters();
                    if (parameters.Length == 0)
                    {
                        _onRemovedElementCallback = (instance, value) =>
                        {
                            onRemovedElementMethod.Invoke(instance, null);
                        };
                    }
                    else if (parameters.Length == 1)
                    {
                        if (!_collectionStructureResolver.ElementType.IsAssignableFrom(parameters[0].ParameterType))
                        {
                            throw new Exception(
                                $"The parameter type of '{_listDrawerSettings.OnRemovedElementCallback}' in '{_listDrawerTargetType}' must be '{_collectionStructureResolver.ElementType}'");
                        }

                        _onRemovedElementCallback = (instance, value) =>
                        {
                            onRemovedElementMethod.Invoke(instance, new object[] { value });
                        };
                    }
                    else
                    {
                        throw new Exception($"The parameter count of '{_listDrawerSettings.OnRemovedElementCallback}' in '{_listDrawerTargetType}' must be 0 or 1");
                    }
                }

                if (_listDrawerSettings.CustomCreateElementFunction.IsNotNullOrEmpty())
                {
                    var customCreateElementFunction =
                        _listDrawerTargetType.GetMethodEx(_listDrawerSettings.CustomCreateElementFunction,
                            BindingFlagsHelper.All)
                        ?? throw new Exception(
                            $"Cannot find method '{_listDrawerSettings.CustomCreateElementFunction}' in '{_listDrawerTargetType}'");

                    _customCreateElementFunction = instance =>
                    {
                        return customCreateElementFunction.Invoke(instance, null);
                    };
                }

                if (_listDrawerSettings.CustomRemoveElementFunction.IsNotNullOrEmpty())
                {
                    var customRemoveElementFunction =
                        _listDrawerTargetType.GetMethodEx(_listDrawerSettings.CustomRemoveElementFunction,
                            BindingFlagsHelper.All)
                        ?? throw new Exception(
                            $"Cannot find method '{_listDrawerSettings.CustomRemoveElementFunction}' in '{_listDrawerTargetType}'");

                    _customRemoveElementFunction = (instance, value) =>
                    {
                        customRemoveElementFunction.Invoke(instance, new object[] { value });
                    };
                }
            }
        }

        private void UpdateLogic()
        {
            if (_orderedCollectionOperation != null)
            {
                if (_removeAt != null && Event.current.type == EventType.Repaint)
                {
                    try
                    {
                        DoRemoveElementAt(_removeAt.Value);
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
                    DoRemoveElement(_removeValues);
                }
                finally
                {
                    _removeValues = null;
                }

                EasyGUIHelper.RequestRepaint();
            }
        }

        protected virtual void DoAddElement(Rect addButtonRect)
        {
            if (_elementDropdownListGetter != null)
            {
                var dropdownItems = _elementDropdownListGetter();
                EasyEditorGUI.ShowValueDropdownMenu(addButtonRect, null, dropdownItems, (item) =>
                {
                    var value = item.GetValue();
                    DoAddElement(value);
                }, (item) => new GUIContent(item.GetText()));
                return;
            }

            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoAddElement(GetValueToAdd(i));
            }
        }

        private void DoAddElement(object valueToAdd)
        {
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoAddElement(i, valueToAdd);
            }
        }

        protected virtual void DoAddElement(int targetIndex, object valueToAdd)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            ValueEntry.EnqueueChange(() =>
            {
                _collectionOperation.AddWeakElement(Property, targetIndex, valueToAdd);
                _onAddedElementCallback?.Invoke(parent, valueToAdd);
            });
        }

        private void DoInsertElement(int index, object valueToAdd)
        {
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoInsertElement(i, index, valueToAdd);
            }
        }

        private void DoInsertElement(int targetIndex, int index, object valueToAdd)
        {
            // Use the new ordered collection operation
            if (_orderedCollectionOperation != null)
            {
                var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
                ValueEntry.EnqueueChange(() =>
                {
                    _orderedCollectionOperation.InsertWeakElementAt(Property, targetIndex, index, valueToAdd);
                    _onAddedElementCallback?.Invoke(parent, valueToAdd);
                });
            }
            else
            {
                throw new InvalidOperationException(
                    $"The property '{Property.Path}' is not ordered collection, so you cannot insert elements at a specific index.");
            }
        }

        protected virtual void DoRemoveElementAt(int index)
        {
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoRemoveElementAt(i, index);
            }
        }

        protected virtual void DoRemoveElementAt(int targetIndex, int index)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            if (_customRemoveIndexFunction != null)
            {
                _customRemoveIndexFunction.Invoke(parent, index);
            }
            else
            {
                Assert.IsTrue(_orderedCollectionOperation != null);
                ValueEntry.EnqueueChange(() =>
                {
                    var valueToRemove = Property.Children[index].ValueEntry.WeakValues[targetIndex];
                    _orderedCollectionOperation.RemoveWeakElementAt(Property, targetIndex, index);
                    _onRemovedElementCallback?.Invoke(parent, valueToRemove);
                });
            }
        }

        protected virtual void DoRemoveElement(InspectorProperty propertyToRemove)
        {
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoRemoveElement(i, propertyToRemove);
            }
        }

        protected virtual void DoRemoveElement(int targetIndex, InspectorProperty propertyToRemove)
        {
            var valueToRemove = propertyToRemove.ValueEntry.WeakValues[targetIndex];
            DoRemoveElement(targetIndex, valueToRemove);
        }

        private void DoRemoveElement(object[] values)
        {
            for (int i = 0; i < Property.Tree.Targets.Count; i++)
            {
                DoRemoveElement(i, values[i]);
            }
        }

        private void DoRemoveElement(int targetIndex, object valueToRemove)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            if (_customRemoveElementFunction != null)
            {
                _customRemoveElementFunction.Invoke(parent, valueToRemove);
            }
            else
            {
                ValueEntry.EnqueueChange(() =>
                {
                    _collectionOperation.RemoveWeakElement(Property, targetIndex, valueToRemove);
                    _onRemovedElementCallback?.Invoke(parent, valueToRemove);
                });
            }
        }

        protected virtual object GetValueToAdd(int targetIndex)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            if (_customCreateElementFunction != null)
            {
                return _customCreateElementFunction.Invoke(parent);
            }

            if (_collectionStructureResolver.ElementType.IsInheritsFrom<UnityEngine.Object>())
            {
                return null;
            }

            return UnitySerializationUtility.CreateDefaultUnityInitializedObject(_collectionStructureResolver.ElementType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Internal
{
    /// <summary>
    /// Manages the lifecycle of Unity Editors and ElementTrees for an EasyEditorWindow.
    /// </summary>
    internal sealed class EditorLifecycleManager : IEditorLifecycleManager
    {
        private static readonly System.Reflection.PropertyInfo MaterialForceVisibleProperty;

        static EditorLifecycleManager()
        {
            MaterialForceVisibleProperty = typeof(MaterialEditor).GetProperty("forceVisible", BindingFlagsHelper.All);
        }

        private object[] _currentTargets = new object[0];
        private UnityEditor.Editor[] _editors = new UnityEditor.Editor[0];
        private IElementTree[] _elementTrees = new IElementTree[0];

        /// <inheritdoc/>
        public IReadOnlyList<object> CurrentTargets => _currentTargets;

        /// <inheritdoc/>
        public IReadOnlyList<UnityEditor.Editor> Editors => _editors;

        /// <inheritdoc/>
        public IReadOnlyList<IElementTree> ElementTrees => _elementTrees;

        /// <inheritdoc/>
        public event Action EditorsUpdated;

        /// <inheritdoc/>
        public void UpdateEditors(IEnumerable<object> targets)
        {
            _currentTargets = _currentTargets ?? new object[] { };
            _editors = _editors ?? new UnityEditor.Editor[] { };
            _elementTrees = _elementTrees ?? new IElementTree[] { };

            IList<object> newTargets = targets?.ToArray() ?? new object[0];

            if (_currentTargets.Length != newTargets.Count)
            {
                AdjustArraySizes(newTargets.Count);
            }

            UpdateTargets(newTargets);
        }

        /// <inheritdoc/>
        public void DestroyAll()
        {
            if (_editors != null)
            {
                for (int i = 0; i < _editors.Length; i++)
                {
                    if (_editors[i])
                    {
                        UnityEngine.Object.DestroyImmediate(_editors[i]);
                        _editors[i] = null;
                    }
                }
            }

            if (_elementTrees != null)
            {
                for (int i = 0; i < _elementTrees.Length; i++)
                {
                    if (_elementTrees[i] != null)
                    {
                        (_elementTrees[i] as IDisposable)?.Dispose();
                        _elementTrees[i] = null;
                    }
                }
            }
        }

        private void AdjustArraySizes(int newSize)
        {
            if (_editors.Length > newSize)
            {
                var toDestroy = _editors.Length - newSize;
                for (int i = 0; i < toDestroy; i++)
                {
                    var e = _editors[_editors.Length - i - 1];
                    if (e) UnityEngine.Object.DestroyImmediate(e);
                }
            }

            if (_elementTrees.Length > newSize)
            {
                var toDestroy = _elementTrees.Length - newSize;
                for (int i = 0; i < toDestroy; i++)
                {
                    var e = _elementTrees[_elementTrees.Length - i - 1];
                    if (e != null) (e as IDisposable)?.Dispose();
                }
            }

            Array.Resize(ref _currentTargets, newSize);
            Array.Resize(ref _editors, newSize);
            Array.Resize(ref _elementTrees, newSize);

            EditorsUpdated?.Invoke();
        }

        private void UpdateTargets(IList<object> newTargets)
        {
            for (int i = 0; i < newTargets.Count; i++)
            {
                var newTarget = newTargets[i];
                var curTarget = _currentTargets[i];
                if (!object.ReferenceEquals(newTarget, curTarget))
                {
                    EasyGUIHelper.RequestRepaint();
                    _currentTargets[i] = newTarget;

                    if (newTarget == null)
                    {
                        ClearTargetAt(i);
                    }
                    else
                    {
                        UpdateTargetAt(i, newTarget);
                    }
                }
            }
        }

        private void ClearTargetAt(int index)
        {
            if (_elementTrees[index] != null)
            {
                (_elementTrees[index] as IDisposable)?.Dispose();
                _elementTrees[index] = null;
            }

            if (_editors[index])
            {
                UnityEngine.Object.DestroyImmediate(_editors[index]);
                _editors[index] = null;
            }
        }

        private void UpdateTargetAt(int index, object target)
        {
            var editorWindow = target as EditorWindow;
            if (target.GetType().IsInheritsFrom<UnityEngine.Object>() && !editorWindow)
            {
                UpdateUnityObjectTarget(index, target as UnityEngine.Object);
            }
            else
            {
                UpdateNonUnityObjectTarget(index, target);
            }
        }

        private void UpdateUnityObjectTarget(int index, UnityEngine.Object unityObject)
        {
            if (_elementTrees[index] != null)
            {
                (_elementTrees[index] as IDisposable)?.Dispose();
                _elementTrees[index] = null;
            }

            if (_editors[index])
            {
                UnityEngine.Object.DestroyImmediate(_editors[index]);
            }

            if (unityObject)
            {
                _editors[index] = UnityEditor.Editor.CreateEditor(unityObject);
                var materialEditor = _editors[index] as MaterialEditor;
                if (materialEditor != null && MaterialForceVisibleProperty != null)
                {
                    MaterialForceVisibleProperty.SetValue(materialEditor, true, null);
                }
            }
            else
            {
                _editors[index] = null;
            }
        }

        private void UpdateNonUnityObjectTarget(int index, object target)
        {
            if (_elementTrees[index] != null)
            {
                (_elementTrees[index] as IDisposable)?.Dispose();
            }

            if (_editors[index])
            {
                UnityEngine.Object.DestroyImmediate(_editors[index]);
            }
            _editors[index] = null;

            if (target is System.Collections.IList list)
            {
                _elementTrees[index] = InspectorElements.TreeFactory.CreateTree(
                    list.Cast<object>().ToArray(), null);
            }
            else
            {
                _elementTrees[index] = InspectorElements.TreeFactory.CreateTree(new[] { target }, null);
            }
        }
    }
}

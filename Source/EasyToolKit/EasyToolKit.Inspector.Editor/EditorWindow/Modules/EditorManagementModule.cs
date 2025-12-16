using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Modules
{
    /// <summary>
    /// Manages Unity editors and property trees creation/destruction
    /// </summary>
    internal class EditorManagementModule : EditorWindowModuleBase
    {
        private static bool hasUpdatedEasyEditors;
        private static System.Reflection.PropertyInfo materialForceVisibleProperty;

        static EditorManagementModule()
        {
            materialForceVisibleProperty = typeof(MaterialEditor).GetProperty("forceVisible", BindingFlagsHelper.All);
        }

        private object[] _currentTargets = new object[0];
        private UnityEditor.Editor[] _editors = new UnityEditor.Editor[0];
        private PropertyTree[] _propertyTrees = new PropertyTree[0];
        public IReadOnlyList<object> CurrentTargets => _currentTargets;
        public UnityEditor.Editor[] Editors => _editors;
        public PropertyTree[] PropertyTrees => _propertyTrees;

        public override void OnGUI()
        {
            // Ensure editors have been updated before rendering
            if (!hasUpdatedEasyEditors)
            {
                InspectorConfigAsset.Instance.EnsureEditorsHaveBeenUpdated();
                hasUpdatedEasyEditors = true;
            }

            if (Event.current.type == EventType.Layout)
            {
                UpdateEditors();
            }
        }

        /// <summary>
        /// Updates the editors and property trees based on current targets
        /// </summary>
        private void UpdateEditors()
        {
            _currentTargets = _currentTargets ?? new object[] { };
            _editors = _editors ?? new UnityEditor.Editor[] { };
            _propertyTrees = _propertyTrees ?? new PropertyTree[] { };

            IList<object> newTargets = Window.GetTargets().ToArray();

            // Handle target count changes
            if (_currentTargets.Length != newTargets.Count)
            {
                CleanupExcessEditors(newTargets.Count);
                ResizeArrays(newTargets.Count);
                Window.Repaint();
            }

            // Update individual targets
            for (int i = 0; i < newTargets.Count; i++)
            {
                UpdateSingleTarget(i, newTargets[i]);
            }
        }

        /// <summary>
        /// Cleans up excess editors when target count decreases
        /// </summary>
        private void CleanupExcessEditors(int newCount)
        {
            if (_editors.Length > newCount)
            {
                var toDestroy = _editors.Length - newCount;
                for (int i = 0; i < toDestroy; i++)
                {
                    var editor = _editors[_editors.Length - i - 1];
                    if (editor != null)
                    {
                        UnityEngine.Object.DestroyImmediate(editor);
                    }
                }
            }

            if (_propertyTrees.Length > newCount)
            {
                var toDestroy = _propertyTrees.Length - newCount;
                for (int i = 0; i < toDestroy; i++)
                {
                    var tree = _propertyTrees[_propertyTrees.Length - i - 1];
                    tree?.Dispose();
                }
            }
        }

        /// <summary>
        /// Resizes all arrays to match the new target count
        /// </summary>
        private void ResizeArrays(int newCount)
        {
            Array.Resize(ref _currentTargets, newCount);
            Array.Resize(ref _editors, newCount);
            Array.Resize(ref _propertyTrees, newCount);
        }

        /// <summary>
        /// Updates a single target at the specified index
        /// </summary>
        private void UpdateSingleTarget(int index, object newTarget)
        {
            var currentTarget = _currentTargets[index];
            if (ReferenceEquals(newTarget, currentTarget)) return;

            EasyGUIHelper.RequestRepaint();
            _currentTargets[index] = newTarget;

            if (newTarget == null)
            {
                CleanupTargetAt(index);
            }
            else
            {
                CreateEditorForTarget(index, newTarget);
            }
        }

        /// <summary>
        /// Cleans up the editor and property tree at the specified index
        /// </summary>
        private void CleanupTargetAt(int index)
        {
            _propertyTrees[index]?.Dispose();
            _propertyTrees[index] = null;

            if (_editors[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(_editors[index]);
                _editors[index] = null;
            }
        }

        /// <summary>
        /// Creates an appropriate editor for the target object
        /// </summary>
        private void CreateEditorForTarget(int index, object target)
        {
            var editorWindow = target as EditorWindow;

            // Handle Unity objects (except EditorWindows)
            if (target.GetType().IsInheritsFrom<UnityEngine.Object>() && editorWindow == null)
            {
                CreateUnityEditor(index, target);
            }
            else
            {
                CreatePropertyTree(index, target);
            }
        }

        /// <summary>
        /// Creates a Unity editor for Unity objects
        /// </summary>
        private void CreateUnityEditor(int index, object target)
        {
            CleanupTargetAt(index);

            var unityObject = target as UnityEngine.Object;
            if (unityObject != null)
            {
                _propertyTrees[index] = null;
                _editors[index] = UnityEditor.Editor.CreateEditor(unityObject);

                // Configure material editor if applicable
                if (_editors[index] is MaterialEditor materialEditor &&
                    materialForceVisibleProperty != null)
                {
                    materialForceVisibleProperty.SetValue(materialEditor, true, null);
                }
            }
            else
            {
                _propertyTrees[index] = null;
                _editors[index] = null;
            }
        }

        /// <summary>
        /// Creates a property tree for non-Unity objects
        /// </summary>
        private void CreatePropertyTree(int index, object target)
        {
            _propertyTrees[index]?.Dispose();
            if (_editors[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(_editors[index]);
                _editors[index] = null;
            }

            if (target is System.Collections.IList list)
            {
                _propertyTrees[index] = PropertyTree.Create(list.Cast<object>().ToArray(), null);
            }
            else
            {
                _propertyTrees[index] = PropertyTree.Create(new[] { target }, null);
            }
        }

        /// <summary>
        /// Draws the editor for the target at the specified index
        /// </summary>
        public void DrawEditor(int index)
        {
            var propertyTree = _propertyTrees[index];
            var editor = _editors[index];

            if (propertyTree != null || (editor != null && editor.target != null))
            {
                if (propertyTree != null)
                {
                    propertyTree.Draw();
                }
                else
                {
                    if (editor is EasyEditor easyEditor)
                    {
                        easyEditor.IsInlineEditor = true;
                    }

                    editor.OnInspectorGUI();
                }
            }
        }

        /// <summary>
        /// Draws the editor preview for the target at the specified index
        /// </summary>
        public void DrawEditorPreview(int index, float height)
        {
            var editor = _editors[index];
            if (editor != null && editor.HasPreviewGUI())
            {
                Rect rect = EditorGUILayout.GetControlRect(false, height);
                editor.DrawPreview(rect);
            }
        }

        /// <summary>
        /// Disposes all editors and property trees
        /// </summary>
        public override void Dispose()
        {
            if (_editors != null)
            {
                for (int i = 0; i < _editors.Length; i++)
                {
                    if (_editors[i] != null)
                    {
                        UnityEngine.Object.DestroyImmediate(_editors[i]);
                        _editors[i] = null;
                    }
                }
            }

            if (_propertyTrees != null)
            {
                for (int i = 0; i < _propertyTrees.Length; i++)
                {
                    _propertyTrees[i]?.Dispose();
                    _propertyTrees[i] = null;
                }
            }
        }
    }
}

using System;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Unity-specific implementation of change management for the inspector system.
    /// Integrates with Unity's Undo system and handles multi-object selection.
    /// </summary>
    public class UnityChangeManager : IChangeManager
    {
        private InspectorProperty _property;
        private Action _changeAction;

        /// <summary>
        /// Initializes a new instance of the UnityChangeManager
        /// </summary>
        /// <param name="property">The inspector property associated with this change manager</param>
        public UnityChangeManager(InspectorProperty property = null)
        {
            _property = property;
        }

        /// <summary>
        /// Gets or sets the associated inspector property
        /// </summary>
        public InspectorProperty Property
        {
            get => _property;
            set => _property = value;
        }

        /// <summary>
        /// Enqueues a change action to be applied later
        /// </summary>
        /// <param name="action">The action representing the change to be applied</param>
        public void EnqueueChange(Action action)
        {
            _changeAction += action;
            _property?.Tree.QueueCallbackUntilRepaint(() =>
            {
                _property?.Tree.SetPropertyDirty(_property);
            });
        }

        /// <summary>
        /// Applies all queued changes
        /// </summary>
        /// <returns>True if changes were applied successfully, false if there were no pending changes</returns>
        public bool ApplyChanges()
        {
            if (_changeAction != null)
            {
                if (_property?.Tree?.Targets != null && _property.Tree.Targets.Count > 0 && _property.Tree.Targets[0] is UnityEngine.Object)
                {
                    foreach (var target in _property.Tree.Targets)
                    {
                        if (target is UnityEngine.Object unityObject)
                        {
                            Undo.RecordObject(unityObject, $"Change {_property.Path} on {unityObject.name}");
                        }
                    }
                }

                _changeAction();
                _changeAction = null;

                _property?.Update();
                if (_property != null)
                {
                    foreach (var child in _property.Children.Recurse())
                    {
                        child.Update();
                    }
                    _property.Children.Clear();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Records an undo operation for the given action
        /// </summary>
        /// <param name="operationName">The name of the operation for undo/redo tracking</param>
        public void RecordUndo(string operationName)
        {
            if (_property?.Tree?.Targets != null && _property.Tree.Targets.Count > 0 && _property.Tree.Targets[0] is UnityEngine.Object)
            {
                foreach (var target in _property.Tree.Targets)
                {
                    if (target is UnityEngine.Object unityObject)
                    {
                        Undo.RecordObject(unityObject, operationName);
                    }
                }
            }
        }
    }
}
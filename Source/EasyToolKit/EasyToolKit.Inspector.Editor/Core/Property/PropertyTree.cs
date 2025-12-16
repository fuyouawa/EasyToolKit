using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a hierarchical tree structure for managing and drawing inspector properties.
    /// This class handles property updates, change tracking, and rendering in the Unity Inspector.
    /// </summary>
    public class PropertyTree
    {
        private readonly HashSet<InspectorProperty> _dirtyProperties = new HashSet<InspectorProperty>();
        private Action _pendingCallbacks;
        private Action _pendingCallbacksUntilRepaint;
        private readonly object[] _targets;

        /// <summary>
        /// Gets the current update identifier used for tracking property updates.
        /// </summary>
        public int UpdateId { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializedObject"/> associated with this property tree.
        /// </summary>
        [CanBeNull] public SerializedObject SerializedObject { get; }

        /// <summary>
        /// Gets the root property of the property tree hierarchy.
        /// </summary>
        public InspectorProperty LogicRootProperty { get; }

        /// <summary>
        /// Gets the target objects associated with this property tree.
        /// </summary>
        public IReadOnlyList<object> Targets => _targets;

        /// <summary>
        /// Gets the type of the target object.
        /// </summary>
        public Type TargetType => LogicRootProperty.Info.PropertyType;

        /// <summary>
        /// Gets or sets whether to draw the <see cref="MonoScript"/> object field in the inspector.
        /// </summary>
        public bool DrawMonoScriptObjectField { get; set; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event Action<InspectorProperty, int> OnPropertyValueChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTree"/> class with the specified <see cref="SerializedObject"/>.
        /// </summary>
        /// <param name="targets">The target objects to create the property tree for.</param>
        /// <param name="serializedObject">The <see cref="SerializedObject"/> to create the property tree for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="serializedObject"/> is null.</exception>
        public PropertyTree([NotNull] object[] targets, [CanBeNull] SerializedObject serializedObject)
        {
            if (targets.IsNullOrEmpty())
                throw new ArgumentException(nameof(targets));

            _targets = targets;
            SerializedObject = serializedObject;
            LogicRootProperty = new InspectorProperty(this, null,
                InspectorPropertyInfo.CreateForLogicRoot(targets), 0);
        }


        /// <summary>
        /// Creates a new <see cref="PropertyTree"/> instance from a <see cref="SerializedObject"/>.
        /// </summary>
        /// <param name="serializedObject">The <see cref="SerializedObject"/> to create the property tree for.</param>
        /// <returns>A new <see cref="PropertyTree"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="serializedObject"/> is null.</exception>
        public static PropertyTree Create([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            return Create(serializedObject.targetObjects, serializedObject);
        }

        /// <summary>
        /// Creates a new <see cref="PropertyTree"/> instance from target objects and an optional <see cref="SerializedObject"/>.
        /// </summary>
        /// <param name="targets">The target objects to create the property tree for.</param>
        /// <param name="serializedObject">An optional existing <see cref="SerializedObject"/> to use.</param>
        /// <returns>A new <see cref="PropertyTree"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="targets"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided <see cref="serializedObject"/> is not valid for the targets.</exception>
        public static PropertyTree Create([NotNull] object[] targets,
            [CanBeNull] SerializedObject serializedObject)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));

            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Length != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (!object.ReferenceEquals(targets[i], targetObjects[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    throw new ArgumentException($"SerializedObject is not valid for targets.");
                }
            }
            else
            {
                // Check if all targets have the same type
                if (targets.Length > 0)
                {
                    Type firstTargetType = targets[0].GetType();
                    bool allSameType = targets.All(t => t.GetType() == firstTargetType);

                    if (!allSameType)
                    {
                        throw new ArgumentException($"All targets must have the same type.");
                    }

                    // Check if the type inherits from UnityEngine.Object
                    if (typeof(UnityEngine.Object).IsAssignableFrom(firstTargetType))
                    {
                        // Convert targets to UnityEngine.Object array
                        var unityObjects = targets.Cast<UnityEngine.Object>().ToArray();
                        serializedObject = new SerializedObject(unityObjects);
                    }
                }
            }

            return new PropertyTree(targets, serializedObject);
        }

        /// <summary>
        /// Enumerates all properties in the tree.
        /// </summary>
        /// <param name="includeChildren">Whether to include child properties in the enumeration.</param>
        /// <returns>An enumerable collection of <see cref="InspectorProperty"/> objects.</returns>
        public IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren)
        {
            for (var i = 0; i < LogicRootProperty.Children!.Count; i++)
            {
                var property = LogicRootProperty.Children[i];
                yield return property;

                if (includeChildren && property.Children != null)
                {
                    foreach (var child in property.Children.Recurse())
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a Unity <see cref="SerializedProperty"/> by its property path.
        /// </summary>
        /// <param name="propertyPath">The path of the property to find.</param>
        /// <returns>The <see cref="SerializedProperty"/> at the specified path, or null if not found.</returns>
        public SerializedProperty GetUnityPropertyByPath(string propertyPath)
        {
            if (SerializedObject == null)
            {
                throw new InvalidOperationException("SerializedObject is null.");
            }
            return SerializedObject.FindProperty(propertyPath);
        }

        /// <summary>
        /// Marks a property as dirty, indicating it needs to be updated.
        /// </summary>
        /// <param name="property">The property to mark as dirty.</param>
        public void SetPropertyDirty(InspectorProperty property)
        {
            _dirtyProperties.Add(property);
        }

        /// <summary>
        /// Queues a callback to be executed during the next update cycle.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        public void QueueCallback(Action action)
        {
            _pendingCallbacks += action;
        }

        /// <summary>
        /// Queues a callback to be executed until the next repaint event.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        public void QueueCallbackUntilRepaint(Action action)
        {
            _pendingCallbacksUntilRepaint += action;
        }

        /// <summary>
        /// Draws the entire property tree in the inspector.
        /// </summary>
        public void Draw()
        {
            BeginDraw();
            DrawProperties();
            EndDraw();
        }

        /// <summary>
        /// Begins the drawing process for the property tree.
        /// This method updates the serialized object and prepares the tree for drawing.
        /// </summary>
        public void BeginDraw()
        {
            SerializedObject?.UpdateIfRequiredOrScript();
            Update();

            if (DrawMonoScriptObjectField && SerializedObject != null)
            {
                var scriptProperty = SerializedObject.FindProperty("m_Script");

                if (scriptProperty != null)
                {
                    var monoScript = scriptProperty.objectReferenceValue as MonoScript;
                    if (monoScript == null)
                    {
                        EasyEditorGUI.MessageBox(
                            "The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.",
                            MessageType.Warning);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(scriptProperty);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        /// <summary>
        /// Draws all properties in the tree.
        /// </summary>
        public void DrawProperties()
        {
            foreach (var property in EnumerateTree(false))
            {
                try
                {
                    property.Draw();
                }
                catch (Exception e)
                {
                    if (e is ExitGUIException || e.InnerException is ExitGUIException)
                    {
                        throw;
                    }

                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Ends the drawing process for the property tree.
        /// This method applies modified properties and processes pending callbacks.
        /// </summary>
        public void EndDraw()
        {
            DoPendingCallbacks();
            SerializedObject?.ApplyModifiedProperties();

            var changed = ApplyChanges();
            if (changed)
            {
                EasyGUIHelper.RequestRepaint();
            }

            DoPendingCallbacks();
            Undo.FlushUndoRecordObjects();
        }

        public void Dispose()
        {
            LogicRootProperty.Dispose();
        }

        private void DoPendingCallbacks()
        {
            if (_pendingCallbacks != null)
            {
                try
                {
                    _pendingCallbacks();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                _pendingCallbacks = null;
            }

            if (_pendingCallbacksUntilRepaint != null)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    try
                    {
                        _pendingCallbacksUntilRepaint();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    _pendingCallbacksUntilRepaint = null;
                }
            }
        }

        private void Update()
        {
            ApplyChanges();
            ++UpdateId;
            LogicRootProperty.Update();
        }


        private bool ApplyChanges()
        {
            bool changed = false;

            int restDirtyPropertiesCount;
            do
            {
                var tempDirtyProperties = new List<InspectorProperty>(_dirtyProperties);
                _dirtyProperties.Clear();
                foreach (var property in tempDirtyProperties)
                {
                    if (property.ChildrenResolver != null)
                    {
                        if (property.ChildrenResolver.ApplyChanges())
                        {
                            changed = true;
                        }
                    }

                    if (property.ValueEntry != null)
                    {
                        if (property.ValueEntry.ApplyChanges())
                        {
                            changed = true;
                        }
                    }
                }

                restDirtyPropertiesCount = _dirtyProperties.Count;
            } while (restDirtyPropertiesCount > 0);

            if (changed)
            {
                if (SerializedObject != null)
                {
                    foreach (var targetObject in SerializedObject.targetObjects)
                    {
                        EasyEditorUtility.SetUnityObjectDirty(targetObject);
                    }
                }
            }

            _dirtyProperties.Clear();
            return changed;
        }

        internal void InvokePropertyValueChanged(InspectorProperty property, int index)
        {
            if (OnPropertyValueChanged != null)
            {
                try
                {
                    OnPropertyValueChanged(property, index);
                }
                catch (ExitGUIException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}

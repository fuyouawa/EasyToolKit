using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="IElementTree"/> that manages the inspector element tree hierarchy.
    /// Handles element creation, drawing, and update coordination for all elements in the inspector.
    /// </summary>
    public class ElementTree : IElementTree
    {
        private readonly object[] _targets;
        private readonly IElementSharedContext _sharedContext;
        private readonly HashSet<IValueElement> _dirtyValueElements = new HashSet<IValueElement>();
        private Action _pendingCallbacks;
        private Action _pendingCallbacksUntilRepaint;
        private bool _disposed;

        /// <summary>
        /// Gets the current update identifier used for tracking element updates.
        /// Increments each time the tree is updated to prevent redundant operations.
        /// </summary>
        public int UpdateId { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializedObject"/> associated with this element tree.
        /// Null if the targets are not Unity objects or don't support serialization.
        /// </summary>
        [CanBeNull]
        public SerializedObject SerializedObject { get; }

        /// <summary>
        /// Gets the root element of the element tree hierarchy.
        /// Represents the top-level container for all inspector elements.
        /// </summary>
        public IRootElement RootElement { get; }

        /// <summary>
        /// Gets the target objects associated with this element tree.
        /// Returns a read-only view of the underlying target array.
        /// </summary>
        public IReadOnlyList<object> Targets => _targets;

        /// <summary>
        /// Gets the type of the target object.
        /// All targets are guaranteed to have the same type.
        /// </summary>
        public Type TargetType => RootElement.Definition.ValueType;

        /// <summary>
        /// Gets or sets whether to draw the <see cref="MonoScript"/> object field in the inspector.
        /// When enabled, displays the script reference at the top of the inspector for MonoBehaviour components.
        /// </summary>
        public bool DrawMonoScriptObjectField { get; set; }

        /// <summary>
        /// Gets the element factory instance owned by this tree.
        /// Provides methods for creating various element types.
        /// </summary>
        public IElementFactory ElementFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementTree"/> class.
        /// </summary>
        /// <param name="targets">The target objects to create the element tree for. Must not be null or empty.</param>
        /// <param name="serializedObject">The optional <see cref="SerializedObject"/> to use for Unity object targets.</param>
        /// <exception cref="ArgumentException">Thrown when targets array is null or empty.</exception>
        public ElementTree([NotNull] object[] targets, [CanBeNull] SerializedObject serializedObject)
        {
            if (targets == null || targets.Length == 0)
                throw new ArgumentException("Targets cannot be null or empty.", nameof(targets));

            _targets = targets;
            SerializedObject = serializedObject;

            // Create shared context with default service container
            _sharedContext = new ElementSharedContext(this);

            // Create element factory
            ElementFactory = new ElementFactory(_sharedContext);

            RootElement = ElementFactory.CreateRootElement(
                InspectorElements.Configurator.Root()
                    .WithValueType(targets[0].GetType())
                    .WithName("$ROOT$")
                    .CreateDefinition());
        }

        /// <summary>
        /// Gets a Unity <see cref="SerializedProperty"/> by its property path.
        /// </summary>
        /// <param name="propertyPath">The path of the property to find.</param>
        /// <returns>The <see cref="SerializedProperty"/> at the specified path, or null if not found or SerializedObject is null.</returns>
        [CanBeNull]
        public SerializedProperty GetUnityPropertyByPath(string propertyPath)
        {
            if (SerializedObject == null)
                return null;

            return SerializedObject.FindProperty(propertyPath);
        }

        /// <summary>
        /// Queues a callback to be executed during the next update cycle.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
        public void QueueCallback(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _pendingCallbacks += action;
        }

        /// <summary>
        /// Queues a callback to be executed until the next repaint event.
        /// The callback will be executed during the next EventType.Repaint and then removed.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
        public void QueueCallbackUntilRepaint(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _pendingCallbacksUntilRepaint += action;
        }

        /// <summary>
        /// Begins the drawing process for the element tree.
        /// Updates the serialized object (if available) and increments the update ID.
        /// Optionally draws the MonoScript field at the top of the inspector.
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
                        EditorGUILayout.HelpBox(
                            "The associated script cannot be loaded.\nPlease fix any compile errors and assign a valid script.",
                            MessageType.Warning);
                    }

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(scriptProperty);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        /// <summary>
        /// Draws all elements in the tree.
        /// Iterates through all top-level elements and invokes their draw methods.
        /// Handles exceptions gracefully and logs them to the Unity console.
        /// </summary>
        public void DrawElements()
        {
            Assert.IsTrue(RootElement.Children != null, "Root element has no children.");
            foreach (var element in RootElement.Children)
            {
                try
                {
                    element.Draw(null);
                }
                catch (Exception e) when (e is ExitGUIException || e.InnerException is ExitGUIException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Ends the drawing process for the element tree.
        /// Processes pending callbacks, applies modified properties, and flushes undo records.
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Disposes the root element and shared context.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            RootElement?.Dispose();
            _sharedContext?.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Updates the element tree by incrementing the update ID.
        /// Elements check this ID to avoid redundant updates within the same frame.
        /// </summary>
        private void Update()
        {
            ApplyChanges();
            ++UpdateId;
            RootElement?.Update();
        }

        private bool ApplyChanges()
        {
            bool changed = false;

            int restDirtyElementsCount;
            do
            {
                var tempDirtyValueElements = new List<IValueElement>(_dirtyValueElements);
                _dirtyValueElements.Clear();
                foreach (var property in tempDirtyValueElements)
                {
                    property.ValueEntry.ApplyChanges();
                    changed = true;
                }

                restDirtyElementsCount = _dirtyValueElements.Count;
            } while (restDirtyElementsCount > 0);

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

            Assert.IsTrue(_dirtyValueElements.Count == 0, "Dirty value elements count is not zero.");
            return changed;
        }

        /// <summary>
        /// Executes all pending callbacks that were queued during the draw cycle.
        /// Handles exceptions gracefully and logs them to the Unity console.
        /// </summary>
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
    }
}

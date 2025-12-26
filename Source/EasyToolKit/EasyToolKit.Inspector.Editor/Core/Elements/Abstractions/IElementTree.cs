using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the entire element tree of an inspector, managing the hierarchy, drawing, and state of all elements.
    /// </summary>
    public interface IElementTree
    {
        /// <summary>
        /// Gets the update identifier that increments every frame.
        /// This value is used to prevent certain logic from executing multiple times within a single frame,
        /// ensuring that operations which should only run once per frame are properly controlled.
        /// </summary>
        int UpdateId { get; }

        /// <summary>
        /// Gets the <see cref="SerializedObject"/> associated with this property tree.
        /// </summary>
        [CanBeNull] public SerializedObject SerializedObject { get; }

        /// <summary>
        /// Gets the root element of the tree.
        /// </summary>
        IRootElement Root { get; }

        /// <summary>
        /// Gets the target objects when multiple objects are selected in the inspector.
        /// </summary>
        public IReadOnlyList<object> Targets { get; }

        public Type TargetType { get; }

        /// <summary>
        /// Gets or sets whether to draw the <see cref="MonoScript"/> object field in the inspector.
        /// </summary>
        public bool DrawMonoScriptObjectField { get; set; }

        /// <summary>
        /// Gets the element factory instance owned by this tree.
        /// </summary>
        IElementFactory ElementFactory { get; }

        /// <summary>
        /// Gets a Unity <see cref="SerializedProperty"/> by its path.
        /// </summary>
        /// <param name="propertyPath">The Unity property path.</param>
        /// <returns>The corresponding serialized property, or <c>null</c> if not found.</returns>
        SerializedProperty GetUnityPropertyByPath(string propertyPath);

        /// <summary>
        /// Queues a callback to be executed during the next update cycle.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        void QueueCallback(Action action);

        /// <summary>
        /// Queues a callback to be executed until the next repaint event.
        /// </summary>
        /// <param name="action">The callback action to queue.</param>
        void QueueCallbackUntilRepaint(Action action);

        /// <summary>
        /// Begins the drawing process for the property tree.
        /// This method updates the serialized object and prepares the tree for drawing.
        /// </summary>
        void BeginDraw();

        /// <summary>
        /// Draws all elements in the tree.
        /// </summary>
        public void DrawElements();

        /// <summary>
        /// Ends the drawing process for the property tree.
        /// This method applies modified properties and processes pending callbacks.
        /// </summary>
        void EndDraw();
    }
}

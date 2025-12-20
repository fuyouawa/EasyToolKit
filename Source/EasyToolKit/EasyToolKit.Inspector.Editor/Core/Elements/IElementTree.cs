using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents the entire element tree of an inspector, managing the hierarchy, drawing, and state of all elements.
    /// </summary>
    public interface IElementTree : IDisposable
    {
        /// <summary>
        /// Gets the current update identifier used for tracking property updates.
        /// </summary>
        int UpdateId { get; }

        /// <summary>
        /// Gets the <see cref="SerializedObject"/> associated with this property tree.
        /// </summary>
        [CanBeNull] public SerializedObject SerializedObject { get; }

        /// <summary>
        /// Gets the root element of the tree.
        /// </summary>
        IRootElement RootElement { get; }

        /// <summary>
        /// Gets the target objects associated with this property tree.
        /// </summary>
        public IReadOnlyList<object> Targets { get; }

        /// <summary>
        /// Gets or sets whether to draw the <see cref="MonoScript"/> object field in the inspector.
        /// </summary>
        public bool DrawMonoScriptObjectField { get; set; }

        /// <summary>
        /// Gets the factory for creating property structure resolvers.
        /// </summary>
        public IPropertyStructureResolverFactory StructureResolverFactory { get; }

        /// <summary>
        /// Gets the factory for creating property operation resolvers.
        /// </summary>
        public IPropertyOperationResolverFactory OperationResolverFactory { get; }

        /// <summary>
        /// Gets the factory for creating attribute resolvers.
        /// </summary>
        public IAttributeResolverFactory AttributeResolverFactory { get; }

        /// <summary>
        /// Gets the factory for creating drawer chain resolvers.
        /// </summary>
        public IDrawerChainResolverFactory DrawerChainResolverFactory { get; }

        /// <summary>
        /// Gets the factory for creating group resolvers.
        /// </summary>
        public IGroupResolverFactory GroupResolverFactory { get; }

        /// <summary>
        /// Enumerates all elements in the tree.
        /// </summary>
        /// <param name="includeChildren">If <c>true</c>, includes all descendant elements; otherwise, only immediate children.</param>
        /// <returns>An enumerable collection of elements.</returns>
        IEnumerable<IElement> EnumerateTree(bool includeChildren);

        /// <summary>
        /// Gets a Unity <see cref="SerializedProperty"/> by its path.
        /// </summary>
        /// <param name="propertyPath">The Unity property path.</param>
        /// <returns>The corresponding serialized property, or <c>null</c> if not found.</returns>
        SerializedProperty GetUnityPropertyByPath(string propertyPath);

        /// <summary>
        /// Marks a property as dirty, indicating it needs to be updated.
        /// </summary>
        /// <param name="property">The property to mark as dirty.</param>
        void SetPropertyDirty(InspectorProperty property);

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
        /// Prepare the drawing process for the property tree.
        /// This method updates the serialized object and prepares the tree for drawing.
        /// </summary>
        void PrepareDraw();

        /// <summary>
        /// Draws all elements in the tree.
        /// </summary>
        public void DrawElements();

        /// <summary>
        /// Finish the drawing process for the property tree.
        /// This method applies modified properties and processes pending callbacks.
        /// </summary>
        void FinishDraw();
    }
}

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines the base contract for all inspector elements in the EasyToolKit inspector system.
    /// This interface serves as the foundational abstraction for all UI elements that can be displayed
    /// in the Unity inspector, providing essential properties and methods for element management,
    /// hierarchy traversal, and rendering.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Gets the definition that describes this element.
        /// </summary>
        IElementDefinition Definition { get; }

        /// <summary>
        /// Gets the element shared context that provides access to tree-level services and resolver factories.
        /// Each element tree has a single shared ElementSharedContext instance that all elements reference.
        /// The context provides dependency injection access to resolver factories and includes an
        /// update identifier used to prevent logic from executing multiple times within a single frame.
        /// </summary>
        IElementSharedContext SharedContext { get; }

        /// <summary>
        /// Gets the current parent element in the element tree hierarchy.
        /// This represents the dynamic parent relationship and automatically updates as elements
        /// are moved, restructured, or modified at runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the current element is in a hover state (removed from its original parent but not yet added to a new parent),
        /// this property returns <c>null</c>.
        /// </para>
        /// </remarks>
        [CanBeNull] IElement Parent { get; }

        /// <summary>
        /// Gets the runtime state of this element.
        /// </summary>
        IElementState State { get; }

        /// <summary>
        /// Gets the hierarchical path of this element.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets or sets the label displayed in the inspector.
        /// </summary>
        GUIContent Label { get; set; }

        /// <summary>
        /// Gets the current phase of this element.
        /// </summary>
        ElementPhases Phases { get; }

        /// <summary>
        /// Gets the runtime child collection for this element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be <c>null</c> for elements that cannot have children.
        /// </para>
        /// </remarks>
        [CanBeNull] IElementList<IElement> Children { get; }

        /// <summary>
        /// Gets all custom attribute infos applied to this value element.
        /// </summary>
        /// <returns>An array of attributes.</returns>
        IReadOnlyList<ElementAttributeInfo> GetAttributeInfos();

        /// <summary>
        /// Tries to get the attribute info for the specified attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute to find.</param>
        /// <param name="attributeInfo">When this method returns, contains the attribute info if found; otherwise, null.</param>
        /// <returns>true if the attribute info was found; otherwise, false.</returns>
        bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo);

        /// <summary>
        /// Requests an action that modifies the tree structure to be executed safely.
        /// If the element is currently drawing, the action is queued and executed after drawing completes.
        /// Otherwise, the action is executed immediately unless <paramref name="forceDelay"/> is <c>true</c>.
        /// </summary>
        /// <param name="action">The action to execute that may modify tree structure.</param>
        /// <param name="forceDelay">
        /// If <c>true</c>, forces the action to be queued and executed after drawing completes, even if not currently drawing.
        /// If <c>false</c>, executes immediately when not drawing.
        /// </param>
        /// <remarks>
        /// Use this method for operations that add, remove, or move elements to prevent modifying
        /// the tree during the drawing phase, which would cause rendering inconsistencies.
        /// </remarks>
        bool Request(Action action, bool forceDelay = false);

        /// <summary>
        /// Sends a message to this element, invoking all marked message handlers.
        /// </summary>
        /// <param name="messageName">The name of the message to send.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// True if at least one handler was found and invoked, false otherwise.
        /// </returns>
        bool Send(string messageName, object args = null);

        /// <summary>
        /// Sends a message and returns the last handler's result.
        /// </summary>
        /// <typeparam name="TResult">The expected return type.</typeparam>
        /// <param name="messageName">The name of the message to send.</param>
        /// <param name="args">Optional arguments to pass to the handlers.</param>
        /// <returns>
        /// The return value from the last handler, or default if no handlers were invoked.
        /// </returns>
        TResult Send<TResult>(string messageName, object args = null);

        /// <summary>
        /// Requests a refresh of the element structure, forcing recreation of resolvers and children.
        /// The refresh occurs through <see cref="Request(Action, bool)"/> to ensure safe execution during drawing.
        /// </summary>
        bool RequestRefresh();

        /// <summary>
        /// Destroys this element, disposing it and removing it from the factory's tracking container.
        /// If the element is not in an idle state, the destruction is queued and executed later.
        /// </summary>
        void Destroy();

        void Update(bool forceUpdate = false);

        bool PostProcessIfNeeded();

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        void Draw(GUIContent label);
    }
}

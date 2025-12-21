using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Defines the base contract for all inspector elements in the EasyToolKit inspector system.
    /// This interface serves as the foundational abstraction for all UI elements that can be displayed
    /// in the Unity inspector, providing essential properties and methods for element management,
    /// hierarchy traversal, and rendering.
    /// </summary>
    public interface IElement : IDisposable
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
        /// Gets the parent element in the inspector tree.
        /// </summary>
        IElement Parent { get; }

        /// <summary>
        /// Gets the logical parent element. If this element is inside a group, this is the <see cref="IGroupElement"/>; otherwise, it equals <see cref="Parent"/>.
        /// </summary>
        IElement LogicParent { get; }

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
        /// Gets the index of this element among its parent's children.
        /// </summary>
        int ChildIndex { get; }

        /// <summary>
        /// Gets the number of frames to skip drawing this element. Decremented by one each time <see cref="Draw(GUIContent)"/> is called (if greater than zero).
        /// </summary>
        int SkipFrameCount { get; }

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        void Draw(GUIContent label);
    }
}

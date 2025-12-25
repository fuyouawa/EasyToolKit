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
        /// Gets the logical parent element that owns this element in the code structure.
        /// This represents the static parent relationship defined by the element's definition and does not change
        /// during runtime modifications or tree restructuring operations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be <c>null</c> for root elements, group elements,
        /// or custom elements created dynamically by users.
        /// </para>
        /// </remarks>
        [CanBeNull] IElement LogicalParent { get; }

        /// <summary>
        /// Gets the current parent element in the element tree hierarchy.
        /// This represents the dynamic parent relationship and automatically updates as elements
        /// are moved, restructured, or modified at runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When an element is initialized, its same as <see cref="LogicalParent"/>.
        /// </para>
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

        [CanBeNull] IReadOnlyElementList<IElement> LogicalChildren { get; }
        [CanBeNull] IElementList<IElement> Children { get; }

        /// <summary>
        /// Gets all custom attribute infos applied to this value element.
        /// </summary>
        /// <returns>An array of attributes.</returns>
        IReadOnlyList<ElementAttributeInfo> GetAttributeInfos();

        DrawerChain GetDrawerChain();

        void Refresh();
        void Update(bool forceUpdate = false);

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        void Draw(GUIContent label);
    }
}

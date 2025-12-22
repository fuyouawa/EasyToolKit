using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Value element interface for all data-containing elements.
    /// Can represent fields, properties, or dynamically created custom values,
    /// and supports independent insertion into the element tree.
    /// </summary>
    public interface IValueElement : IElement
    {
        /// <summary>
        /// Gets the value definition that describes this value element.
        /// </summary>
        new IValueDefinition Definition { get; }

        /// <summary>
        /// Gets the child elements defined by the code structure.
        /// These are immutable and determined solely by the definition.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references,
        /// or when <see cref="IFieldDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </para>
        /// <para>
        /// If the current element is a container (Collection), then LogicalChildren contains all the container's Item elements
        /// (all of which are of type <see cref="ICollectionItemElement"/>), and will automatically change as items are modified.
        /// </para>
        /// </remarks>
        [CanBeNull] IReadOnlyElementList<IElement> LogicalChildren { get; }

        /// <summary>
        /// Gets child elements that were added or removed at runtime (e.g., by user interaction).
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references,
        /// or when <see cref="IFieldDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </para>
        /// <para>
        /// Children will also contain all the container's Item elements. If the modified element is an <see cref="ICollectionItemElement"/>,
        /// it will simultaneously update the container's actual backend Value.
        /// It also supports insertion of custom elements (non-<see cref="ICollectionItemElement"/> elements will not be added to the actual backend Value).
        /// </para>
        /// </remarks>
        [CanBeNull] IElementList<IElement> Children { get; }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// </summary>
        IValueEntry ValueEntry { get; }
    }
}

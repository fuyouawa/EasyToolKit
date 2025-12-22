using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Value element interface for all data-containing elements.
    /// Can represent fields, properties, or dynamically created custom values, and supports independent insertion into the element tree.
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
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references, or when <see cref="IFieldDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </remarks>
        [CanBeNull] IReadOnlyElementList LogicalChildren { get; }

        /// <summary>
        /// Gets child elements that were added or removed at runtime (e.g., by user interaction).
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references, or when <see cref="IFieldDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </remarks>
        [CanBeNull] IElementList Children { get; }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// </summary>
        IValueEntry ValueEntry { get; }
    }
}

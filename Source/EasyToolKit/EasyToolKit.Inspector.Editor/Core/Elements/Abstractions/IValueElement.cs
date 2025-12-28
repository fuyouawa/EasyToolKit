using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Value element interface for all data-containing elements.
    /// Can represent fields, properties, or dynamically created custom values,
    /// and supports independent insertion into the element tree.
    /// </summary>
    public interface IValueElement : ILogicalElement
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
        /// </remarks>
        [CanBeNull] new IReadOnlyElementList<ILogicalElement> LogicalChildren { get; }

        /// <summary>
        /// Gets child elements that were added or removed at runtime (e.g., by user interaction).
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references,
        /// or when <see cref="IFieldDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </para>
        /// </remarks>
        [CanBeNull] new IElementList<IElement> Children { get; }

        /// <summary>
        /// Gets the base value entry that is built directly from <see cref="IValueDefinition.ValueType"/>.
        /// This represents the declared type of the value.
        /// </summary>
        IValueEntry BaseValueEntry { get; }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// This is built based on the runtime type of the value in <see cref="BaseValueEntry"/>.
        /// </summary>
        /// <remarks>
        /// <para>When the runtime type equals the declared type, this is the same as <see cref="BaseValueEntry"/>.</para>
        /// <para>When the runtime type is a derived type, this is a type wrapper around <see cref="BaseValueEntry"/>.</para>
        /// </remarks>
        IValueEntry ValueEntry { get; }
    }
}

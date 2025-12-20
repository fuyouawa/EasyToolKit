using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a value element in the inspector tree. A value element can be a field, property, or method.
    /// </summary>
    public interface IValueElement : IElement
    {
        /// <summary>
        /// Gets the value definition that describes this value element.
        /// </summary>
        new IValueDefinition Definition { get; }

        /// <summary>
        /// Gets whether this value element is read‑only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the child elements of this value (fields, properties, or dynamic virtual value).
        /// Returns <c>null</c> for plain value types, for <see cref="UnityEngine.Object"/> references, or when <see cref="IPropertyDefinition.AsUnityProperty"/> is <c>true</c>.
        /// </summary>
        [CanBeNull] IElementCollection Children { get; }

        /// <summary>
        /// Gets the value entry that manages the underlying value storage and change notifications.
        /// </summary>
        IValueEntry ValueEntry { get; }

        /// <summary>
        /// Gets all custom attributes applied to this value element.
        /// </summary>
        /// <returns>An array of attributes.</returns>
        Attribute[] GetAttributes();
    }
}

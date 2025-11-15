using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection of property values for multiple targets in the inspector.
    /// This interface provides access to property values across all inspected objects.
    /// </summary>
    public interface IPropertyValueCollection : IEnumerable, IDisposable
    {
        /// <summary>
        /// Gets or sets the property value at the specified target index.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The property value for the specified target.</returns>
        object this[int index] { get; set; }

        /// <summary>
        /// Gets the number of target objects in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the inspector property associated with this value collection.
        /// </summary>
        InspectorProperty Property { get; }

        /// <summary>
        /// Gets a value indicating whether any values in this collection have been modified.
        /// </summary>
        bool Dirty { get; }

        /// <summary>
        /// Forces the collection to be marked as dirty, indicating that values have been modified.
        /// </summary>
        void ForceMakeDirty();

        /// <summary>
        /// Updates the collection with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        void Update();

        /// <summary>
        /// Applies any pending changes from this collection back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        bool ApplyChanges();
    }

    /// <summary>
    /// Represents a strongly-typed collection of property values for multiple targets in the inspector.
    /// This generic interface provides type-safe access to property values across all inspected objects.
    /// </summary>
    /// <typeparam name="T">The type of the property values.</typeparam>
    public interface IPropertyValueCollection<T> : IPropertyValueCollection, IReadOnlyList<T>
    {
        /// <summary>
        /// Gets or sets the strongly-typed property value at the specified target index.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The strongly-typed property value for the specified target.</returns>
        new T this[int index] { get; set; }

        /// <summary>
        /// Gets the number of target objects in this collection.
        /// </summary>
        new int Count { get; }
    }
}
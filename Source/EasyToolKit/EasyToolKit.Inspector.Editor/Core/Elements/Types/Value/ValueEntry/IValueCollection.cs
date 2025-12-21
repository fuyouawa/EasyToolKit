using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a collection of property values across multiple target objects.
    /// </summary>
    public interface IValueCollection : IEnumerable
    {
        /// <summary>
        /// Gets the value element associated with this collection.
        /// </summary>
        IValueElement Element { get; }

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
        /// Gets a value indicating whether any values in this collection have been modified.
        /// </summary>
        bool Dirty { get; }

        /// <summary>
        /// Forces the collection to be marked as dirty, indicating that values have been modified.
        /// </summary>
        void ForceMakeDirty();

        /// <summary>
        /// Updates the collection with current values from the target objects.
        /// </summary>
        void Update();

        /// <summary>
        /// Applies any pending changes from this collection back to the target objects.
        /// </summary>
        /// <returns>True if changes were applied; otherwise, false.</returns>
        bool ApplyChanges();
    }

    /// <summary>
    /// Represents a strongly‑typed collection of property values across multiple target objects.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    public interface IValueCollection<TValue> : IValueCollection, IReadOnlyList<TValue>
    {
        /// <summary>
        /// Gets or sets the strongly-typed property value at the specified target index.
        /// </summary>
        /// <param name="index">The zero-based index of the target object.</param>
        /// <returns>The strongly-typed property value for the specified target.</returns>
        new TValue this[int index] { get; set; }

        /// <summary>
        /// Gets the number of target objects in this collection.
        /// </summary>
        new int Count { get; }
    }
}

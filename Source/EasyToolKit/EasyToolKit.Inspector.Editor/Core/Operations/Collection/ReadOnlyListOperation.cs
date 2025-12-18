using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Operation for IReadOnlyList{T} collections in the inspector system.
    /// Provides read-only access to collection elements and prevents modifications.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement IReadOnlyList{TElement})</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class ReadOnlyListOperation<TOwner, TCollection, TElement> : CollectionOperation<TOwner, TCollection, TElement>
        where TCollection : IReadOnlyList<TElement>
    {
        public ReadOnlyListOperation([NotNull] IPropertyOperation<TOwner, TCollection> auxiliaryOperation) : base(auxiliaryOperation)
        {
        }

        /// <summary>
        /// Gets whether the collection is read-only (always returns true)
        /// </summary>
        public override bool IsReadOnly => true;

        public override void AddElement(ref TCollection collection, TElement value)
        {
            throw new NotSupportedException("Modifying a read-only list is not supported.");
        }

        public override void RemoveElement(ref TCollection collection, TElement value)
        {
            throw new NotSupportedException("Modifying a read-only list is not supported.");
        }
    }
}

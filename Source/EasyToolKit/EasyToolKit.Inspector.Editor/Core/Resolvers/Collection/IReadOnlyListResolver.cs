using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Resolver for <see cref="IReadOnlyList{T}"/> collections in the inspector.
    /// Provides read-only access to collection elements and prevents modifications.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement <see cref="IReadOnlyList{TElement}"/>).</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public class IReadOnlyListResolver<TCollection, TElement> : CollectionResolverBase<TCollection, TElement>
        where TCollection : IReadOnlyList<TElement>
    {
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        /// <summary>
        /// Clears cached property information when the resolver is deinitialized.
        /// </summary>
        protected override void Deinitialize()
        {
            _propertyInfosByIndex.Clear();
        }

        /// <summary>
        /// Converts a child name to an index. Not supported for read-only list resolvers.
        /// </summary>
        /// <param name="name">The child name to convert.</param>
        /// <returns>Throws NotSupportedException.</returns>
        /// <exception cref="NotSupportedException">Always thrown as this operation is not supported.</exception>
        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculates the number of child properties (elements) in the collection.
        /// Uses the minimum count across all selected objects in multi-selection.
        /// </summary>
        /// <returns>The minimum number of elements across all selected collections.</returns>
        public override int CalculateChildCount()
        {
            var minLength = int.MaxValue;
            foreach (var value in ValueEntry.Values)
            {
                if (value == null)
                {
                    return 0;
                }
                minLength = Mathf.Min(minLength, value.Count);
            }
            return minLength;
        }

        /// <summary>
        /// Gets the property information for a child element at the specified index.
        /// Provides read-only access to collection elements.
        /// </summary>
        /// <param name="childIndex">The index of the child element.</param>
        /// <returns>The property information for the child element.</returns>
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForValue(
                ElementType,
                $"Array.data[{childIndex}]",
                new GenericValueAccessor<TCollection, TElement>(
                    (list) => list[childIndex],
                    (list, value) => throw new NotSupportedException("Readonly list cannot be modified")
                )
            );

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        /// <summary>
        /// Attempts to add an element to the collection. Always throws NotSupportedException.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <exception cref="NotSupportedException">Always thrown as read-only lists cannot be modified.</exception>
        protected override void AddElement(int targetIndex, TElement value)
        {
            throw new NotSupportedException("Readonly list cannot be modified");
        }

        /// <summary>
        /// Attempts to remove an element from the collection. Always throws NotSupportedException.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        /// <exception cref="NotSupportedException">Always thrown as read-only lists cannot be modified.</exception>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            throw new NotSupportedException("Readonly list cannot be modified");
        }

        /// <summary>
        /// Determines if the collection is read-only. Always returns true for <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection instance to check.</param>
        /// <returns>Always returns true.</returns>
        protected override bool IsReadOnlyCollection(TCollection collection)
        {
            return true;
        }
    }
}

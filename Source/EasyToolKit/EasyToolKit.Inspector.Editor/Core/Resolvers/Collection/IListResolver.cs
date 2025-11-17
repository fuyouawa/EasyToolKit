using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Resolver for <see cref="IList{T}"/> collections in the inspector.
    /// Provides implementation for list-specific operations like indexed access and insertion.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection (must implement <see cref="IList{TElement}"/>).</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection.</typeparam>
    public class IListResolver<TCollection, TElement> : OrderedCollectionResolverBase<TCollection, TElement>
        where TCollection : IList<TElement>
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
        /// Converts a child name to an index. Not supported for list resolvers.
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
        /// Uses caching to improve performance for repeated access.
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
                    (list, value) => list[childIndex] = value
                )
            );

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        /// <summary>
        /// Adds an element to the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to add to the collection.</param>
        protected override void AddElement(int targetIndex, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Add(value);
        }

        /// <summary>
        /// Inserts an element into the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection where the element should be inserted.</param>
        /// <param name="value">The value to insert into the collection.</param>
        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Insert(index, value);
        }

        /// <summary>
        /// Removes an element from the collection at the specified target index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="value">The value to remove from the collection.</param>
        protected override void RemoveElement(int targetIndex, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Remove(value);
        }

        /// <summary>
        /// Removes an element from the collection at a specific index.
        /// </summary>
        /// <param name="targetIndex">The index of the target object in a multi-selection context.</param>
        /// <param name="index">The position in the collection from which to remove the element.</param>
        protected override void RemoveElementAt(int targetIndex, int index)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.RemoveAt(index);
        }
    }
}

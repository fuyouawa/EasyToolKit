using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class IListResolver<TCollection, TElement> : OrderedCollectionResolverBase<TCollection, TElement>
        where TCollection : IList<TElement>
    {
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        protected override void Deinitialize()
        {
            _propertyInfosByIndex.Clear();
        }

        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException();
        }

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

        protected override void AddElement(int targetIndex, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Add(value);
        }

        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Insert(index, value);
        }

        protected override void RemoveElement(int targetIndex, TElement value)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.Remove(value);
        }

        protected override void RemoveElementAt(int targetIndex, int index)
        {
            var collection = ValueEntry.Values[targetIndex];
            collection.RemoveAt(index);
        }
    }
}

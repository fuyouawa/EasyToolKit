using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class CollectionStructureResolverBase<TCollection> : PropertyStructureResolverBase<TCollection>, ICollectionStructureResolver
    {
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public abstract Type ElementType { get; }

        /// <summary>
        /// Gets information about a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Information about the child property</returns>
        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForCollectionElement(
                ElementType,
                $"Array.data[{childIndex}]",
                childIndex,
                typeof(TCollection)
            );

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        /// <summary>
        /// Converts a child property name to its index (not supported for collections)
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>Throws NotSupportedException</returns>
        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException("Collection resolvers do not support name-based access");
        }
    }
}

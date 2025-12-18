using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class CollectionStructureResolverBase<TCollection, TElement> : PropertyStructureResolverBase<TCollection>, ICollectionStructureResolver
        where TCollection : IEnumerable<TElement>
    {
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        private IPropertyValueEntry<TCollection> _valueEntry;

        /// <summary>
        /// Gets the property value entry for the collection, with lazy initialization
        /// </summary>
        protected IPropertyValueEntry<TCollection> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    if (_valueEntry == null)
                    {
                        Property.Update(true);
                        _valueEntry = Property.ValueEntry as IPropertyValueEntry<TCollection>;
                    }
                }
                return _valueEntry;
            }
        }

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public Type ElementType => typeof(TElement);

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

        /// <summary>
        /// Clears cached property information when the resolver is deinitialized
        /// </summary>
        protected override void Deinitialize()
        {
            _propertyInfosByIndex.Clear();
        }
    }
}

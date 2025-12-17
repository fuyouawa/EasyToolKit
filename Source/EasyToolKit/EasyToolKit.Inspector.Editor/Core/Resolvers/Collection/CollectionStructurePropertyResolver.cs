using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Core collection structure resolver that handles property structure resolution for collections
    /// without any collection operations or change management concerns.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection</typeparam>
    /// <typeparam name="TElement">The type of elements in the collection</typeparam>
    public class CollectionStructurePropertyResolver<TCollection, TElement> : PropertyStructureResolver, ICollectionStructureResolver
        where TCollection : IEnumerable<TElement>
    {
        private readonly ICollectionOperationResolver _operationResolver;
        private readonly IChangeManager _changeManager;

        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        private IPropertyValueEntry<TCollection> _valueEntry;

        /// <summary>
        /// Initializes a new instance of the CollectionStructureCore
        /// </summary>
        /// <param name="operationResolver">The collection operation resolver for metadata (read-only status, types)</param>
        public CollectionStructurePropertyResolver(ICollectionOperationResolver operationResolver, IChangeManager changeManager)
        {
            _operationResolver = operationResolver ?? throw new ArgumentNullException(nameof(operationResolver));
            _changeManager = changeManager;
        }

        /// <summary>
        /// Gets the property value entry for the collection, with lazy initialization
        /// </summary>
        private IPropertyValueEntry<TCollection> ValueEntry
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
        /// Gets whether the collection is read-only
        /// </summary>
        public bool IsReadOnly => _operationResolver.IsReadOnly;

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        public Type CollectionType => _operationResolver.CollectionType;

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public Type ElementType => _operationResolver.ElementType;

        public IChangeManager ChangeManager => _changeManager;
        public ICollectionOperationResolver OperationResolver => _operationResolver;

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

            info = InspectorPropertyInfo.CreateForValue(
                ElementType,
                $"Array.data[{childIndex}]",
                new GenericValueAccessor<TCollection, TElement>(
                    (collection) => (TElement)_operationResolver.GetElementAt(collection, childIndex),
                    (collection, value) => _operationResolver.SetElementAt(collection, childIndex, value)
                )
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
        /// Calculates the number of child properties in the collection
        /// </summary>
        /// <returns>The number of elements in the collection</returns>
        public override int CalculateChildCount()
        {
            var minLength = int.MaxValue;
            foreach (var value in ValueEntry.Values)
            {
                if (value == null)
                {
                    return 0;
                }

                var count = GetElementCount((TCollection)value);
                minLength = Math.Min(minLength, count);
            }
            return minLength == int.MaxValue ? 0 : minLength;
        }

        /// <summary>
        /// Clears cached property information when the resolver is deinitialized
        /// </summary>
        protected override void Deinitialize()
        {
            _propertyInfosByIndex.Clear();
        }

        /// <summary>
        /// Gets the element count for a collection
        /// </summary>
        /// <param name="collection">The collection to count</param>
        /// <returns>The number of elements in the collection</returns>
        protected virtual int GetElementCount(TCollection collection)
        {
            if (collection is ICollection<TElement> collectionInterface)
            {
                return collectionInterface.Count;
            }

            if (collection is IReadOnlyCollection<TElement> readOnlyCollection)
            {
                return readOnlyCollection.Count;
            }

            if (collection is ICollection nonGenericCollection)
            {
                return nonGenericCollection.Count;
            }

            throw new NotSupportedException($"Collection type {typeof(TCollection)} does not support counting");
        }
    }
}

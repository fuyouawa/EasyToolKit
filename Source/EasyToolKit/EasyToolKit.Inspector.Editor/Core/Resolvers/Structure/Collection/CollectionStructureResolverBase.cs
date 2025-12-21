using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(1.0)]
    public abstract class CollectionStructureResolverBase<TCollection> : ValueStructureResolverBase<TCollection>, ICollectionStructureResolver
    {
        private readonly Dictionary<int, IElementDefinition> _definitionsByIndex =
            new Dictionary<int, IElementDefinition>();

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public abstract Type ItemType { get; }

        /// <summary>
        /// Gets the definition of a child property at the specified index
        /// </summary>
        /// <param name="childIndex">The index of the child property</param>
        /// <returns>Definition of the child property</returns>
        protected override IElementDefinition GetChildDefinition(int childIndex)
        {
            if (_definitionsByIndex.TryGetValue(childIndex, out var definition))
            {
                return definition;
            }

            definition = InspectorElements.Configurator.CollectionItem()
                .WithItemIndex(childIndex)
                .WithValueType(ItemType)
                .WithName($"Array.data[{childIndex}]")
                .CreateDefinition();

            _definitionsByIndex[childIndex] = definition;
            return definition;
        }

        /// <summary>
        /// Converts a child property name to its index (not supported for collections)
        /// </summary>
        /// <param name="name">The name of the child property</param>
        /// <returns>Throws NotSupportedException</returns>
        protected override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException("Collection resolvers do not support name-based access");
        }

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Flags.IsCollection();
        }
    }
}

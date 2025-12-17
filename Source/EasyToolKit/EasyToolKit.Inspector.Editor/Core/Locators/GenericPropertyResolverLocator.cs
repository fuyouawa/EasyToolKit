using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property resolver locator that handles generic collection types and provides
    /// specialized resolvers using the new composition-based architecture.
    /// </summary>
    /// <remarks>
    /// This locator specifically handles:
    /// - <see cref="IList{T}"/> collections with composition-based resolvers
    /// - <see cref="IReadOnlyList{T}"/> collections with composition-based resolvers
    /// - Other types with GenericPropertyStructureResolver
    /// </remarks>
    public class GenericPropertyResolverLocator : PropertyResolverLocator
    {
        /// <summary>
        /// Gets the appropriate property resolver for generic collection types using composition.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>
        /// An IPropertyResolver instance based on the collection type:
        /// - CollectionPropertyResolver for <see cref="IList{T}"/> collections
        /// - CollectionPropertyResolver for <see cref="IReadOnlyList{T}"/> collections
        /// - GenericPropertyStructureResolver for other types
        /// </returns>
        public override IPropertyStructureResolver GetResolver(InspectorProperty property)
        {
            var type = property.ValueEntry.ValueType;

            // Handle generic enumerable types
            if (type.IsImplementsOpenGenericType(typeof(IEnumerable<>)))
            {
                var elementType = type.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];

                // Use composition-based resolver for mutable list collections
                if (type.IsImplementsOpenGenericType(typeof(IList<>)))
                {
                    return CreateListCollectionResolver(property, type, elementType);
                }

                // Use composition-based resolver for read-only list collections
                if (type.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
                {
                    return CreateReadOnlyListCollectionResolver(property, type, elementType);
                }
            }

            // Default to generic property structure resolver for non-collection types
            return new GenericPropertyStructureResolver();
        }

        /// <summary>
        /// Creates a composition-based collection resolver for IList{T} collections
        /// </summary>
        /// <param name="property">The inspector property</param>
        /// <param name="collectionType">The collection type</param>
        /// <param name="elementType">The element type</param>
        /// <returns>A composition-based collection resolver</returns>
        private IPropertyStructureResolver CreateListCollectionResolver(InspectorProperty property, Type collectionType, Type elementType)
        {
            // Create the operation resolver
            var operationResolverType = typeof(ListOperationResolver<,>).MakeGenericType(collectionType, elementType);
            var operationResolver = operationResolverType.CreateInstance<ICollectionOperationResolver>(property.ValueEntry);

            // Create the change manager
            var changeManager = new UnityChangeManager(property);

            // Create the resolver
            var resolverType = typeof(CollectionStructurePropertyResolver<,>).MakeGenericType(collectionType, elementType);
            return resolverType.CreateInstance<IPropertyStructureResolver>(operationResolver, changeManager);
        }

        /// <summary>
        /// Creates a composition-based collection resolver for IReadOnlyList{T} collections
        /// </summary>
        /// <param name="property">The inspector property</param>
        /// <param name="collectionType">The collection type</param>
        /// <param name="elementType">The element type</param>
        /// <returns>A composition-based collection resolver</returns>
        private IPropertyStructureResolver CreateReadOnlyListCollectionResolver(InspectorProperty property, Type collectionType, Type elementType)
        {
            // Create the operation resolver
            var operationResolverType = typeof(ReadOnlyListOperationResolver<,>).MakeGenericType(collectionType, elementType);
            var operationResolver = operationResolverType.CreateInstance<ICollectionOperationResolver>(property.ValueEntry);

            // Create the change manager
            var changeManager = new UnityChangeManager(property);

            // Create the resolver
            var resolverType = typeof(CollectionStructurePropertyResolver<,>).MakeGenericType(collectionType, elementType);
            return resolverType.CreateInstance<IPropertyStructureResolver>(operationResolver, changeManager);
        }
    }
}

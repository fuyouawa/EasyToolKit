using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property resolver locator that handles generic collection types and provides
    /// specialized resolvers for different collection interfaces.
    /// </summary>
    /// <remarks>
    /// This locator specifically handles:
    /// - <see cref="IList{T}"/> collections with IListResolver
    /// - <see cref="IReadOnlyList{T}"/> collections with IReadOnlyListResolver
    /// - <see cref="IEnumerable{T}"/> types with GenericPropertyResolver
    /// </remarks>
    public class GenericPropertyResolverLocator : PropertyResolverLocator
    {
        /// <summary>
        /// Gets the appropriate property resolver for generic collection types.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>
        /// An IPropertyResolver instance based on the collection type:
        /// - IListResolver for <see cref="IList{T}"/> collections
        /// - IReadOnlyListResolver for <see cref="IReadOnlyList{T}"/> collections
        /// - GenericPropertyResolver for other types
        /// </returns>
        public override IPropertyResolver GetResolver(InspectorProperty property)
        {
            var type = property.ValueEntry.ValueType;

            // Handle generic enumerable types
            if (type.IsImplementsOpenGenericType(typeof(IEnumerable<>)))
            {
                var elementType = type.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];

                // Use IListResolver for mutable list collections
                if (type.IsImplementsOpenGenericType(typeof(IList<>)))
                {
                    return typeof(IListResolver<,>).MakeGenericType(type, elementType).CreateInstance<IPropertyResolver>();
                }

                // Use IReadOnlyListResolver for read-only list collections
                if (type.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
                {
                    return typeof(IReadOnlyListResolver<,>).MakeGenericType(type, elementType).CreateInstance<IPropertyResolver>();
                }
            }

            // Default to generic property resolver for non-collection types
            return new GenericPropertyResolver();
        }
    }
}

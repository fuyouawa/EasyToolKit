using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property resolver locator specifically designed for Unity serialized properties
    /// and collections, providing specialized resolvers that work with Unity's serialization system.
    /// </summary>
    /// <remarks>
    /// This locator handles:
    /// - Unity collections with composite CollectionPropertyResolver
    /// - Regular Unity properties with UnityPropertyStructureResolver
    /// It leverages Unity's SerializedProperty system for proper serialization support.
    /// </remarks>
    public class UnityPropertyResolverLocator : PropertyResolverLocator
    {
        /// <summary>
        /// Gets the appropriate property resolver for Unity serialized properties.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>
        /// An IPropertyStructureResolver instance based on the property type:
        /// - CollectionPropertyResolver for collection types
        /// - UnityPropertyStructureResolver for other property types
        /// </returns>
        public override IPropertyStructureResolver GetResolver(InspectorProperty property)
        {
            if (property?.ValueEntry == null)
                throw new ArgumentException("Property and ValueEntry cannot be null", nameof(property));

            var valueType = property.ValueEntry.ValueType;
            var serializedProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);

            // Handle Unity collections (arrays and lists)
            if (IsUnityCollectionType(valueType, serializedProperty))
            {
                return CreateCollectionResolver(property, valueType);
            }

            // Default to Unity property structure resolver for non-collection types
            return new UnityPropertyStructureResolver();
        }

        /// <summary>
        /// Determines if the type represents a Unity collection type
        /// </summary>
        /// <param name="valueType">The value type to check</param>
        /// <param name="serializedProperty">The serialized property for additional context</param>
        /// <returns>True if this is a Unity collection type</returns>
        private static bool IsUnityCollectionType(Type valueType, SerializedProperty serializedProperty)
        {
            // Check for Unity array serialized property
            if (serializedProperty != null && serializedProperty.isArray)
                return true;

            // Check for generic collection interfaces
            return valueType.IsImplementsOpenGenericType(typeof(ICollection<>)) ||
                   valueType.IsImplementsOpenGenericType(typeof(IList<>)) ||
                   valueType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)) ||
                   typeof(IList).IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Creates a composite collection resolver for Unity collections
        /// </summary>
        /// <param name="property">The inspector property</param>
        /// <param name="valueType">The value type of the collection</param>
        /// <returns>A configured CollectionPropertyResolver instance</returns>
        private static IPropertyStructureResolver CreateCollectionResolver(InspectorProperty property, Type valueType)
        {
            // Determine the element type
            Type elementType = GetElementType(valueType);

            // Create Unity-specific operation resolver
            var operationResolverType = typeof(UnityCollectionOperationResolver<>).MakeGenericType(elementType);
            var operationResolver = operationResolverType.CreateInstance<ICollectionOperationResolver>(property);

            // Create Unity change manager
            var changeManager = new UnityChangeManager(property);

            // Create the resolver
            var resolverType = typeof(CollectionStructurePropertyResolver<,>).MakeGenericType(valueType, elementType);
            return resolverType.CreateInstance<IPropertyStructureResolver>(operationResolver, changeManager);
        }

        /// <summary>
        /// Determines the element type for a collection
        /// </summary>
        /// <param name="valueType">The collection value type</param>
        /// <returns>The element type of the collection</returns>
        private static Type GetElementType(Type valueType)
        {
            // Try to get element type from generic collection interfaces
            if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                return valueType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];
            }

            if (valueType.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                return valueType.GetArgumentsOfInheritedOpenGenericType(typeof(IList<>))[0];
            }

            if (valueType.IsImplementsOpenGenericType(typeof(IReadOnlyList<>)))
            {
                return valueType.GetArgumentsOfInheritedOpenGenericType(typeof(IReadOnlyList<>))[0];
            }

            // For non-generic IList, we can't determine the element type statically
            // This is a limitation that would need runtime information
            throw new NotSupportedException($"Cannot determine element type for collection type: {valueType.FullName}");
        }
    }
}

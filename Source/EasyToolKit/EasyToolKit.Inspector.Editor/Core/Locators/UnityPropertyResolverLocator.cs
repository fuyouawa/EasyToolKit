using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property resolver locator specifically designed for Unity serialized properties
    /// and collections, providing specialized resolvers that work with Unity's serialization system.
    /// </summary>
    /// <remarks>
    /// This locator handles:
    /// - Unity collections (<see cref="ICollection{T}"/>) with UnityCollectionResolver
    /// - Regular Unity properties with UnityPropertyResolver
    /// It leverages Unity's SerializedProperty system for proper serialization support.
    /// </remarks>
    public class UnityPropertyResolverLocator : PropertyResolverLocator
    {
        /// <summary>
        /// Gets the appropriate property resolver for Unity serialized properties.
        /// </summary>
        /// <param name="property">The inspector property to find a resolver for.</param>
        /// <returns>
        /// An IPropertyResolver instance based on the property type:
        /// - UnityCollectionResolver for <see cref="ICollection{T}"/> types
        /// - UnityPropertyResolver for other property types
        /// </returns>
        public override IPropertyResolver GetResolver(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;
            var serializedProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);

            // Handle Unity collections
            if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                // Ensure we're dealing with a Unity array property
                Assert.IsTrue(serializedProperty.isArray);
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                // Create accessor type for the collection
                var accessorType = typeof(UnityCollectionAccessor<,,>)
                    .MakeGenericType(property.Parent.ValueEntry.ValueType, valueType, elementType);

                // Return specialized resolver for Unity collections
                return typeof(UnityCollectionResolver<>).MakeGenericType(elementType).CreateInstance<IPropertyResolver>();
            }
            else
            {
                // Default to Unity property resolver for non-collection types
                return new UnityPropertyResolver();
            }
        }
    }
}

using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Factory interface responsible for creating <see cref="IElement"/> instances based on their definitions.
    /// </summary>
    /// <remarks>
    /// The factory is owned by an <see cref="IElementTree"/> instance and provides centralized element creation
    /// with consistent initialization through the shared context.
    /// </remarks>
    public interface IElementFactory
    {
        /// <summary>
        /// Creates a value element from the given value definition.
        /// </summary>
        /// <param name="definition">The value definition describing the value element to create.</param>
        /// <returns>A new value element instance.</returns>
        [NotNull] IValueElement CreateValueElement([NotNull] IValueDefinition definition);

        /// <summary>
        /// Creates a field element from the given field definition.
        /// </summary>
        /// <param name="definition">The field definition describing the field element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field element instance.</returns>
        [NotNull] IFieldElement CreateFieldElement([NotNull] IFieldDefinition definition, [CanBeNull] IValueElement parent);

        /// <summary>
        /// Creates a property element from the given property definition.
        /// </summary>
        /// <param name="definition">The property definition describing the property element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property element instance.</returns>
        [NotNull] IPropertyElement CreatePropertyElement([NotNull] IPropertyDefinition definition, [CanBeNull] IValueElement parent);

        /// <summary>
        /// Creates a group element from the given group definition.
        /// </summary>
        /// <param name="definition">The group definition describing the group to create.</param>
        /// <returns>A new group element instance.</returns>
        [NotNull] IGroupElement CreateGroupElement([NotNull] IGroupDefinition definition);

        /// <summary>
        /// Creates a method element from the given method definition.
        /// </summary>
        /// <param name="definition">The method definition describing the method to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new method element instance.</returns>
        [NotNull] IMethodElement CreateMethodElement([NotNull] IMethodDefinition definition, [CanBeNull] ILogicalElement parent);

        /// <summary>
        /// Creates a method parameter element from the given method parameter definition.
        /// </summary>
        /// <param name="definition">The method parameter definition describing the parameter element to create.</param>
        /// <param name="parent">The optional logical parent method element that contains this parameter.</param>
        /// <returns>A new method parameter element instance.</returns>
        [NotNull] IMethodParameterElement CreateMethodParameterElement([NotNull] IMethodParameterDefinition definition, [CanBeNull] IMethodElement parent);

        /// <summary>
        /// Creates a collection element from the given collection definition.
        /// </summary>
        /// <param name="definition">The collection definition describing the collection to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new collection element instance.</returns>
        [NotNull] ICollectionElement CreateCollectionElement([NotNull] ICollectionDefinition definition, [CanBeNull] ILogicalElement parent);

        /// <summary>
        /// Creates a property collection element from the given property collection definition.
        /// </summary>
        /// <param name="definition">The property collection definition describing the property collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property collection element instance.</returns>
        [NotNull] IPropertyCollectionElement CreatePropertyCollectionElement([NotNull] IPropertyCollectionDefinition definition, [CanBeNull] IValueElement parent);

        /// <summary>
        /// Creates a field collection element from the given field collection definition.
        /// </summary>
        /// <param name="definition">The field collection definition describing the field collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field collection element instance.</returns>
        [NotNull] IFieldCollectionElement CreateFieldCollectionElement([NotNull] IFieldCollectionDefinition definition, [CanBeNull] IValueElement parent);

        /// <summary>
        /// Creates a collection item element from the given collection item definition.
        /// </summary>
        /// <param name="definition">The collection item definition describing the collection item to create.</param>
        /// <param name="parent">The optional logical parent collection element that contains this item.</param>
        /// <returns>A new collection item element instance.</returns>
        [NotNull] ICollectionItemElement CreateCollectionItemElement([NotNull] ICollectionItemDefinition definition, [CanBeNull] ICollectionElement parent);

        /// <summary>
        /// Creates a root element from the given root definition.
        /// </summary>
        /// <param name="definition">The root definition describing the root element to create.</param>
        /// <returns>A new root element instance.</returns>
        [NotNull] IRootElement CreateRootElement([NotNull] IRootDefinition definition);

        /// <summary>
        /// Destroys the specified element, disposing it and removing it from the factory's tracking container.
        /// If the element is not in an idle state, the destruction is queued and executed later.
        /// </summary>
        /// <param name="element">The element to destroy.</param>
        /// <returns><c>true</c> if the element was successfully destroyed or queued for destruction; <c>false</c> if the element was not found in the tracking container.</returns>
        bool DestroyElement([NotNull] IElement element);
    }
}

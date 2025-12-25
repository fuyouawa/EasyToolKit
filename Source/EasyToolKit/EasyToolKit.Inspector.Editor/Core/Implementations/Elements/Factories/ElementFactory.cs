using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default factory implementation for creating <see cref="IElement"/> instances based on their definitions.
    /// </summary>
    public class ElementFactory : IElementFactory
    {
        private const string ErrorDefinitionNull = "Element definition cannot be null.";
        private readonly IElementSharedContext _sharedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementFactory"/> class.
        /// </summary>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sharedContext"/> is null.</exception>
        public ElementFactory([NotNull] IElementSharedContext sharedContext)
        {
            _sharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));
        }

        /// <summary>
        /// Creates a value element from the given value definition.
        /// </summary>
        /// <param name="definition">The value definition describing the value element to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new value element instance.</returns>
        public IValueElement CreateValueElement(IValueDefinition definition, IElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new ValueElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a field element from the given field definition.
        /// </summary>
        /// <param name="definition">The field definition describing the field element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field element instance.</returns>
        public IFieldElement CreateFieldElement(IFieldDefinition definition, IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new FieldElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a property element from the given property definition.
        /// </summary>
        /// <param name="definition">The property definition describing the property element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property element instance.</returns>
        public IPropertyElement CreatePropertyElement(IPropertyDefinition definition, IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new PropertyElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a group element from the given group definition.
        /// </summary>
        /// <param name="definition">The group definition describing the group to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new group element instance.</returns>
        public IGroupElement CreateGroupElement(IGroupDefinition definition, IElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new GroupElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a method element from the given method definition.
        /// </summary>
        /// <param name="definition">The method definition describing the method to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        public IMethodElement CreateMethodElement(IMethodDefinition definition, IElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new MethodElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a method parameter element from the given method parameter definition.
        /// </summary>
        /// <param name="definition">The method parameter definition describing the parameter element to create.</param>
        /// <param name="parent">The optional logical parent method element that contains this parameter.</param>
        /// <returns>A new method parameter element instance.</returns>
        public IMethodParameterElement CreateMethodParameterElement(IMethodParameterDefinition definition, IMethodElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new MethodParameterElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a collection element from the given collection definition.
        /// </summary>
        /// <param name="definition">The collection definition describing the collection to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new collection element instance.</returns>
        public ICollectionElement CreateCollectionElement(ICollectionDefinition definition, IElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new CollectionElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a property collection element from the given property collection definition.
        /// </summary>
        /// <param name="definition">The property collection definition describing the property collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property collection element instance.</returns>
        public IPropertyCollectionElement CreatePropertyCollectionElement(IPropertyCollectionDefinition definition, IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new PropertyCollectionElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a field collection element from the given field collection definition.
        /// </summary>
        /// <param name="definition">The field collection definition describing the field collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field collection element instance.</returns>
        public IFieldCollectionElement CreateFieldCollectionElement(IFieldCollectionDefinition definition, IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new FieldCollectionElement(definition, _sharedContext, parent);
        }

        /// <summary>
        /// Creates a collection item element from the given collection item definition.
        /// </summary>
        /// <param name="definition">The collection item definition describing the collection item to create.</param>
        /// <param name="parent">The optional logical parent collection element that contains this item.</param>
        /// <returns>A new collection item element instance.</returns>
        public ICollectionItemElement CreateCollectionItemElement(ICollectionItemDefinition definition, ICollectionElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new CollectionItemElement(definition, _sharedContext, parent);
        }

        public IRootElement CreateRootElement(IRootDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return new RootElement(definition, _sharedContext);
        }
    }
}

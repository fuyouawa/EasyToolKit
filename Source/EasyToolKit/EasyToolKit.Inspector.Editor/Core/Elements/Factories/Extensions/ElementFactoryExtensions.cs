using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class ElementFactoryExtensions
    {
        [NotNull]
        public static IElement CreateElement(this IElementFactory factory, [NotNull] IElementDefinition definition, [CanBeNull] ILogicalElement parent)
        {
            if (definition is IFieldCollectionDefinition fieldCollectionDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return factory.CreateFieldCollectionElement(fieldCollectionDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for field collection definition", nameof(parent));
            }

            if (definition is IPropertyCollectionDefinition propertyCollectionDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return factory.CreatePropertyCollectionElement(propertyCollectionDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for property collection definition", nameof(parent));
            }

            if (definition is IFieldDefinition fieldDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return factory.CreateFieldElement(fieldDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for field definition", nameof(parent));
            }

            if (definition is IPropertyDefinition propertyDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return factory.CreatePropertyElement(propertyDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for property definition", nameof(parent));
            }

            if (definition is ICollectionItemDefinition collectionItemDefinition)
            {
                if (parent is ICollectionElement || parent is null)
                {
                    return factory.CreateCollectionItemElement(collectionItemDefinition, (ICollectionElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a collection element for collection item definition", nameof(parent));
            }

            if (definition is IMethodParameterDefinition methodParameterDefinition)
            {
                if (parent is IMethodElement || parent is null)
                {
                    return factory.CreateMethodParameterElement(methodParameterDefinition, (IMethodElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a method element for method parameter definition", nameof(parent));
            }

            if (definition is IRootDefinition rootDefinition)
            {
                return factory.CreateRootElement(rootDefinition);
            }

            if (definition is IMethodDefinition methodDefinition)
            {
                return factory.CreateMethodElement(methodDefinition, parent);
            }

            if (definition is IGroupDefinition groupDefinition)
            {
                return factory.CreateGroupElement(groupDefinition);
            }

            if (definition is ICollectionDefinition collectionDefinition)
            {
                return factory.CreateCollectionElement(collectionDefinition, parent);
            }

            if (definition is IValueDefinition valueDefinition)
            {
                return factory.CreateValueElement(valueDefinition);
            }

            throw new ArgumentException($"Definition '{definition}' is not a valid element definition", nameof(definition));
        }
    }
}

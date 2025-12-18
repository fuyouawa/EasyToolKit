using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverLocatorPriority(0.0)]
    public class GenericPropertyStructureResolverLocator : PropertyStructureResolverLocator
    {
        public override bool CanResolver(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;

            if (valueType.IsInheritsFrom<IEnumerable>())
            {
                return valueType.IsImplementsOpenGenericType(typeof(ICollection<>)) ||
                       valueType.IsImplementsOpenGenericType(typeof(IReadOnlyCollection<>));
            }

            return true;
        }

        public override IPropertyStructureResolver GetResolver(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;

            if (valueType.IsInheritsFrom<IEnumerable>())
            {
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];

                Type resolverType;
                if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
                {
                    resolverType = typeof(CollectionStructureResolver<,>).MakeGenericType(valueType, elementType);
                }
                else if (valueType.IsImplementsOpenGenericType(typeof(IReadOnlyCollection<>)))
                {
                    resolverType = typeof(ReadOnlyCollectionStructureResolver<,>).MakeGenericType(valueType, elementType);
                }
                else
                {
                    throw new NotSupportedException();
                }
                return resolverType.CreateInstance<IPropertyStructureResolver>();
            }

            // Default to generic property structure resolver for non-collection types
            return new GenericPropertyStructureResolver();
        }
    }
}

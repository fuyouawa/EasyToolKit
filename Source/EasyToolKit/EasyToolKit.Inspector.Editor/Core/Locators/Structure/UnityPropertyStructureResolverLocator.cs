using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverLocatorPriority(-10000.0)]
    public class UnityPropertyStructureResolverLocator : PropertyStructureResolverLocator
    {
        public override bool CanResolver(InspectorProperty property)
        {
            var serializedProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);
            if (serializedProperty == null)
                return false;

            var valueType = property.ValueEntry.ValueType;

            if (valueType.IsInheritsFrom<IEnumerable>())
            {
                return valueType.IsImplementsOpenGenericType(typeof(IList<>));
            }

            return true;
        }

        public override IPropertyStructureResolver GetResolver(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;

            if (valueType.IsInheritsFrom<IEnumerable>())
            {
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(IEnumerable<>))[0];

                var resolverType = typeof(UnityCollectionStructureResolver<,>).MakeGenericType(valueType, elementType);
                return resolverType.CreateInstance<IPropertyStructureResolver>();
            }

            return new UnityPropertyStructureResolver();
        }
    }
}

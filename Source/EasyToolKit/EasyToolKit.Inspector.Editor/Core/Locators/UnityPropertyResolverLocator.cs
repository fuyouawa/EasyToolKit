using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityPropertyResolverLocator : PropertyResolverLocator
    {
        public override IPropertyResolver GetResolver(InspectorProperty property)
        {
            var valueType = property.ValueEntry.ValueType;
            var serializedProperty = property.Tree.GetUnityPropertyByPath(property.UnityPath);
            if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                Assert.IsTrue(serializedProperty.isArray);
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                var accessorType = typeof(UnityCollectionAccessor<,,>)
                    .MakeGenericType(property.Parent.ValueEntry.ValueType, valueType, elementType);
                return typeof(UnityCollectionResolver<>).MakeGenericType(elementType).CreateInstance<IPropertyResolver>();
            }
            else
            {
                return new UnityPropertyResolver();
            }
        }
    }
}
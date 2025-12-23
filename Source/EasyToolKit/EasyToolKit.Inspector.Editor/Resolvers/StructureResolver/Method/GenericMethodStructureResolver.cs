using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericMethodStructureResolver : MethodStructureResolverBase
    {
        private readonly List<IMethodParameterDefinition> _definitions = new List<IMethodParameterDefinition>();
        private ParameterInfo[] _parameterInfos;

        protected override bool CanResolveElement(IMethodElement element)
        {
            var parameterInfos = element.Definition.MethodInfo.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType.IsGenericType)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void Initialize()
        {
            _parameterInfos = Element.Definition.MethodInfo.GetParameters();
            for (var i = 0; i < _parameterInfos.Length; i++)
            {
                var parameterInfo = _parameterInfos[i];
                _definitions.Add(
                    InspectorElements.Configurator.MethodParameter()
                        .WithParameterInfo(parameterInfo)
                        .WithParameterIndex(i)
                        .CreateDefinition());
            }
        }

        protected override IElementDefinition GetChildDefinition(int childIndex)
        {
            return _definitions[childIndex];
        }

        protected override int ChildNameToIndex(string name)
        {
            for (var i = 0; i < _definitions.Count; i++)
            {
                var definition = _definitions[i];
                if (definition.Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        protected override int CalculateChildCount()
        {
            return _definitions.Count;
        }
    }
}

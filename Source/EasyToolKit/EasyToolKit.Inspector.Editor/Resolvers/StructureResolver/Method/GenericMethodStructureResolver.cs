using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override IElementDefinition[] GetChildrenDefinitions()
        {
            return _definitions.Cast<IElementDefinition>().ToArray();
        }

        /// <summary>
        /// Clears the cached parameter definitions and parameter infos when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _definitions.Clear();
            _parameterInfos = null;
        }
    }
}

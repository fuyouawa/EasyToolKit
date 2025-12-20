using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorResolverUtility
    {
        public static Type GetResolverType(InspectorProperty property, Type resolverBaseType)
        {
            var resolverType = InspectorElementUtility.GetElementTypes(property, type => type.IsInheritsFrom(resolverBaseType)).FirstOrDefault();
            if (resolverType == null)
            {
                throw new Exception($"Cannot get resolver type '{resolverBaseType}' for property '{property}'");
            }

            return resolverType;
        }
    }
}

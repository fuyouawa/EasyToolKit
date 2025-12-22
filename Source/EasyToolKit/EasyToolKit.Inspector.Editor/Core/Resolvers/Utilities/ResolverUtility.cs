using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public static class ResolverUtility
    {
        public static Type GetResolverType(IElement element, Type resolverBaseType)
        {
            var resolverType = HandlerUtility.GetElementTypes(element, type => type.IsInheritsFrom(resolverBaseType)).FirstOrDefault();
            if (resolverType == null)
            {
                throw new Exception($"Cannot get resolver type '{resolverBaseType}' for property '{element}'");
            }

            return resolverType;
        }
    }
}

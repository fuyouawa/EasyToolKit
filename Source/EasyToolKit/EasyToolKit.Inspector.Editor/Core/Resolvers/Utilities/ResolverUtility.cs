using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class ResolverUtility
    {
        [CanBeNull]
        public static Type GetResolverType(IElement element, Type resolverBaseType)
        {
            var resolverType = HandlerUtility.GetElementTypes(element, type => type.IsInheritsFrom(resolverBaseType)).FirstOrDefault();
            return resolverType;
        }
    }
}

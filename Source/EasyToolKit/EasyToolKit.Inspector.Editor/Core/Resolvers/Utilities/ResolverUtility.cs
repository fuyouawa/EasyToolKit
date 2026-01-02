using System;
using System.Collections.Concurrent;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class ResolverUtility
    {
        /// <summary>
        /// Cache for Type.GetInterfaces() to avoid repeated allocations.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type[]> InterfacesCache = new ConcurrentDictionary<Type, Type[]>();

        /// <summary>
        /// Rents a resolver of the specified type from the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <param name="type">The type of resolver to rent.</param>
        /// <returns>A rented resolver instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when type is null.</exception>
        public static IResolver RentResolver([NotNull] Type type)
        {
            return EditorPoolUtility.RentAs<IResolver>(type);
        }

        /// <summary>
        /// Releases a resolver back to the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <param name="resolver">The resolver to release.</param>
        /// <exception cref="ArgumentNullException">Thrown when resolver is null.</exception>
        public static void ReleaseResolver(IResolver resolver)
        {
            EditorPoolUtility.ReleaseAs(resolver);
        }

        /// <summary>
        /// Gets the resolver type from an element based on the resolver base type.
        /// </summary>
        /// <param name="element">The element to search for resolver type.</param>
        /// <param name="resolverBaseType">The base resolver type (interface or class).</param>
        /// <returns>The first resolver type found, or null if none found.</returns>
        [CanBeNull]
        public static Type GetResolverType(IElement element, Type resolverBaseType)
        {
            return HandlerUtility.GetFirstElementType(element, type =>
            {
                if (resolverBaseType.IsInterface)
                {
                    var interfaces = InterfacesCache.GetOrAdd(type, t => t.GetInterfaces());
                    foreach (var iface in interfaces)
                    {
                        if (iface == resolverBaseType)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return type.IsAssignableFrom(resolverBaseType);
            });
        }
    }
}

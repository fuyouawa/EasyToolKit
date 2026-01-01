using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class ResolverUtility
    {
        private static readonly ConcurrentDictionary<Type, Func<IResolver>> RentResolverCache = new ConcurrentDictionary<Type, Func<IResolver>>();

        private static readonly ConcurrentDictionary<Type, Action<IResolver>> ReleaseResolverCache = new ConcurrentDictionary<Type, Action<IResolver>>();

        /// <summary>
        /// Cache for Type.GetInterfaces() to avoid repeated allocations.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type[]> InterfacesCache = new ConcurrentDictionary<Type, Type[]>();

        private static class ResolverPoolFastGetter<TResolver>
            where TResolver : class, IResolver, new()
        {
            public static readonly IObjectPool<TResolver> Pool;

            static ResolverPoolFastGetter()
            {
                if (!ObjectPoolManager.TryGetPool(typeof(TResolver).FullName, out Pool))
                {
                    Pool = ObjectPoolManager.CreatePool<TResolver>(typeof(TResolver).FullName);
                }
            }
        }

        private static readonly IObjectPoolManager ObjectPoolManager = Pools.ManagerFactory.CreateObjectPoolManager();

        /// <summary>
        /// Rents a resolver of the specified type from the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <param name="type">The type of resolver to rent.</param>
        /// <returns>A rented resolver instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when type is null.</exception>
        public static IResolver RentResolver([NotNull] Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var rentFunc = RentResolverCache.GetOrAdd(type, t =>
            {
                var rentResolverWrapperMethod = typeof(ResolverUtility)
                    .GetMethod(nameof(RentResolverWrapper), BindingFlagsHelper.AllStatic)!
                    .MakeGenericMethod(t);

                var callExpression = Expression.Call(rentResolverWrapperMethod);
                var lambda = Expression.Lambda<Func<IResolver>>(callExpression);

                return lambda.Compile();
            });

            return rentFunc();
        }

        /// <summary>
        /// Releases a resolver back to the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <param name="resolver">The resolver to release.</param>
        /// <exception cref="ArgumentNullException">Thrown when resolver is null.</exception>
        public static void ReleaseResolver(IResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var resolverType = resolver.GetType();

            var releaseAction = ReleaseResolverCache.GetOrAdd(resolverType, t =>
            {
                var releaseResolverWrapperMethod = typeof(ResolverUtility)
                    .GetMethod(nameof(ReleaseResolverWrapper), BindingFlagsHelper.AllStatic)!
                    .MakeGenericMethod(t);

                var parameter = Expression.Parameter(typeof(IResolver), "resolver");
                var convert = Expression.Convert(parameter, t);
                var callExpression = Expression.Call(releaseResolverWrapperMethod, convert);
                var lambda = Expression.Lambda<Action<IResolver>>(callExpression, parameter);

                return lambda.Compile();
            });

            releaseAction(resolver);
        }

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

        private static TResolver RentResolverWrapper<TResolver>() where TResolver : class, IResolver, new()
        {
            return ResolverPoolFastGetter<TResolver>.Pool.Rent();
        }

        private static void ReleaseResolverWrapper<TResolver>(TResolver resolver) where TResolver : class, IResolver, new()
        {
            ResolverPoolFastGetter<TResolver>.Pool.Release(resolver);
        }
    }
}

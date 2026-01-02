
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace EasyToolKit.Core.Editor
{
    public static class EditorPoolUtility
    {
        private static readonly IObjectPoolManager ObjectPoolManager = Pools.ManagerFactory.CreateObjectPoolManager();

        /// <summary>
        /// Fast getter for object pools with generic type constraint.
        /// </summary>
        /// <typeparam name="T">The type of object in the pool (must be class and have parameterless constructor).</typeparam>
        private static class PoolFastGetter<T>
            where T : class, new()
        {
            public static readonly IObjectPool<T> Pool;

            static PoolFastGetter()
            {
                if (!ObjectPoolManager.TryGetPool(typeof(T).FullName, out Pool))
                {
                    Pool = ObjectPoolManager.CreatePool<T>(typeof(T).FullName);
                }
            }
        }

        /// <summary>
        /// Rents an object of the specified type from the object pool.
        /// </summary>
        /// <typeparam name="T">The type of object to rent (must be class and have parameterless constructor).</typeparam>
        /// <returns>A rented object instance.</returns>
        public static T Rent<T>() where T : class, new()
        {
            return PoolFastGetter<T>.Pool.Rent();
        }

        /// <summary>
        /// Releases an object back to the object pool.
        /// </summary>
        /// <typeparam name="T">The type of object to release.</typeparam>
        /// <param name="instance">The object instance to release.</param>
        /// <exception cref="ArgumentNullException">Thrown when instance is null.</exception>
        public static void Release<T>(T instance) where T : class, new()
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            PoolFastGetter<T>.Pool.Release(instance);
        }

        /// <summary>
        /// Rents an object as the specified interface type from the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to return.</typeparam>
        /// <param name="type">The concrete type implementing the interface (must have parameterless constructor).</param>
        /// <returns>A rented object instance cast to the interface type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when type is null.</exception>
        public static TInterface RentAs<TInterface>(Type type) where TInterface : class
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var rentFunc = RentCache<TInterface>.Cache.GetOrAdd(type, t =>
            {
                var rentWrapperMethod = typeof(EditorPoolUtility)
                    .GetMethod(nameof(RentWrapper), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(t);

                var callExpression = Expression.Call(rentWrapperMethod);
                var lambda = Expression.Lambda<Func<TInterface>>(callExpression);

                return lambda.Compile();
            });

            return rentFunc();
        }

        /// <summary>
        /// Releases an object back to the object pool using compiled Expression for fast invocation.
        /// </summary>
        /// <typeparam name="TInterface">The interface type of the object.</typeparam>
        /// <param name="instance">The object instance to release.</param>
        /// <exception cref="ArgumentNullException">Thrown when instance is null.</exception>
        public static void ReleaseAs<TInterface>(TInterface instance) where TInterface : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var instanceType = instance.GetType();

            var releaseAction = ReleaseCache<TInterface>.Cache.GetOrAdd(instanceType, t =>
            {
                var releaseWrapperMethod = typeof(EditorPoolUtility)
                    .GetMethod(nameof(ReleaseWrapper), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(t);

                var parameter = Expression.Parameter(typeof(TInterface), "instance");
                var convert = Expression.Convert(parameter, t);
                var callExpression = Expression.Call(releaseWrapperMethod, convert);
                var lambda = Expression.Lambda<Action<TInterface>>(callExpression, parameter);

                return lambda.Compile();
            });

            releaseAction(instance);
        }

        private static TRent RentWrapper<TRent>() where TRent : class, new()
        {
            return PoolFastGetter<TRent>.Pool.Rent();
        }

        private static void ReleaseWrapper<TRelease>(TRelease instance) where TRelease : class, new()
        {
            PoolFastGetter<TRelease>.Pool.Release(instance);
        }

        private static class RentCache<TInterface> where TInterface : class
        {
            public static readonly ConcurrentDictionary<Type, Func<TInterface>> Cache = new ConcurrentDictionary<Type, Func<TInterface>>();
        }

        private static class ReleaseCache<TInterface> where TInterface : class
        {
            public static readonly ConcurrentDictionary<Type, Action<TInterface>> Cache = new ConcurrentDictionary<Type, Action<TInterface>>();
        }
    }
}

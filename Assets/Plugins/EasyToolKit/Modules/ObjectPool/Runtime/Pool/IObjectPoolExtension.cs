using System;

namespace EasyToolKit.ToolKit
{
    /// <summary>
    /// 对象池扩展方法类，提供了一系列便捷的对象池操作方法
    /// </summary>
    public static class IObjectPoolExtension
    {
        /// <summary>
        /// 添加对象生成时的回调函数
        /// </summary>
        /// <typeparam name="TObjectPool">对象池类型</typeparam>
        /// <param name="objectPool">目标对象池</param>
        /// <param name="callback">对象生成时的回调函数</param>
        /// <returns>对象池实例，支持链式调用</returns>
        public static TObjectPool OnRent<TObjectPool>(this TObjectPool objectPool, Action<object> callback)
            where TObjectPool : IObjectPool
        {
            objectPool.AddRentCallback(callback);
            return objectPool;
        }

        /// <summary>
        /// 添加对象回收时的回调函数
        /// </summary>
        /// <typeparam name="TObjectPool">对象池类型</typeparam>
        /// <param name="objectPool">目标对象池</param>
        /// <param name="callback">对象回收时的回调函数</param>
        /// <returns>对象池实例，支持链式调用</returns>
        public static TObjectPool OnRelease<TObjectPool>(this TObjectPool objectPool, Action<object> callback)
            where TObjectPool : IObjectPool
        {
            objectPool.AddReleaseCallback(callback);
            return objectPool;
        }

        /// <summary>
        /// 设置对象池的容量
        /// </summary>
        /// <typeparam name="TObjectPool">对象池类型</typeparam>
        /// <param name="objectPool">目标对象池</param>
        /// <param name="newCapacity">新的容量值</param>
        /// <returns>对象池实例，支持链式调用</returns>
        public static TObjectPool SetCapacity<TObjectPool>(this TObjectPool objectPool, int newCapacity)
            where TObjectPool : IObjectPool
        {
            objectPool.Capacity = newCapacity;
            return objectPool;
        }

        /// <summary>
        /// 从对象池中获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">期望的对象类型</typeparam>
        /// <param name="objectPool">目标对象池</param>
        /// <returns>获取到的对象</returns>
        /// <exception cref="ArgumentException">当期望的类型与对象池中的对象类型不兼容时抛出</exception>
        public static T Rent<T>(this IObjectPool objectPool)
        {
            var obj = objectPool.Rent();
            
            if (obj is T o)
            {
                return o;
            }

            throw new ArgumentException($"Generic type '{typeof(T)}' is not assignable from object pool type '{objectPool.ObjectType}'.");
        }
    }
}

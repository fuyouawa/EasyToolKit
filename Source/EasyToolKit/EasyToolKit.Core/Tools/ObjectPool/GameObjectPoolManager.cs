using System;
using UnityEngine;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    /// <summary>
    /// 游戏对象池管理器，负责创建和管理多个游戏对象池
    /// </summary>
    public class GameObjectPoolManager : IGameObjectPoolManager
    {
        // 使用元组作为键，存储游戏对象池实例
        private readonly Dictionary<string, IGameObjectPool> _pools =
            new Dictionary<string, IGameObjectPool>();

        public Transform Transform { get; set; }

        void FixedUpdate()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Update(Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// 创建游戏对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="original">原始预制体</param>
        /// <exception cref="InvalidOperationException">当已存在同名同类型的池时抛出</exception>
        public IGameObjectPool CreatePool(string poolName, GameObject original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            if (_pools.ContainsKey(poolName))
            {
                throw new InvalidOperationException(
                    $"Game object pool '{poolName}' already exists.");
            }

            var pool = CreateUnityObjectPool(poolName, original);

            // 创建一个节点作为此 Pool 的父级，用于层级管理
            var node = new GameObject(poolName);
            node.transform.SetParent(Transform, false);
            pool.Transform = node.transform;

            _pools.Add(poolName, pool);
            return pool;
        }

        public bool HasPool(string poolName)
        {
            return _pools.ContainsKey(poolName);
        }

        /// <summary>
        /// 获取指定名称和类型的游戏对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <returns>找到的对象池</returns>
        /// <exception cref="InvalidOperationException">当找不到指定的对象池时抛出</exception>
        public IGameObjectPool GetPool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                return pool;
            }

            throw new InvalidOperationException(
                $"Game object pool '{poolName}' does not exist.");
        }

        /// <summary>
        /// 创建新的游戏对象池实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="original">原始预制体</param>
        /// <returns></returns>
        private IGameObjectPool CreateUnityObjectPool(string poolName, GameObject original)
        {
            IGameObjectPool pool = new GameObjectPool();
            pool.Name = poolName;
            pool.ObjectType = typeof(GameObject);
            pool.Original = original;
            return pool;
        }
    }
}

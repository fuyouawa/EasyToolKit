using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    public interface IGameObjectPoolManager
    {
        Transform Transform { get; set; }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="original">用于实例化的原始游戏对象</param>
        /// <exception cref="InvalidOperationException">当已存在同名同类型的池时抛出</exception>
        IGameObjectPool CreatePool(string poolName, GameObject original);

        bool HasPool(string poolName);

        /// <summary>
        /// 获取指定名称和类型的对象池
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <returns>找到的对象池</returns>
        /// <exception cref="InvalidOperationException">当找不到指定的对象池时抛出</exception>
        IGameObjectPool GetPool(string poolName);
    }
}

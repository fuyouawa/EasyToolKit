using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    public enum PooledUnityObjectState
    {
        Avtive,
        Idle,
    }

    class PooledGameObject
    {
        /// <summary>
        /// 目标实例
        /// </summary>
        public GameObject Target { get; }

        /// <summary>
        /// 所属池
        /// </summary>
        public IGameObjectPool OwningPool { get; }

        /// <summary>
        /// 激活中对象的回收时间，小于0则无限制
        /// </summary>
        public float ActiveLifetime { get; set; }

        /// <summary>
        /// 空闲中对象的销毁时间，小于0则无限制
        /// </summary>
        public float IdleLifetime { get; set; }

        /// <summary>
        /// <para>获取当前计时器累计时间（单位：秒）。</para>
        /// <para>- 激活状态时：表示已激活时长</para>
        /// <para>- 空闲状态时：表示已空闲时长</para>
        /// </summary>
        public float ElapsedTime { get; set; }

        public PooledUnityObjectState State { get; set; }

        public PooledGameObject(GameObject target, IGameObjectPool owningPool)
        {
            Target = target;
            OwningPool = owningPool;
        }
    }

    /// <summary>
    /// 游戏对象池实现，用于管理游戏对象的对象池
    /// </summary>
    public class GameObjectPool : ObjectPoolBase, IGameObjectPool
    {
        private GameObject _original;

        private readonly Dictionary<GameObject, PooledGameObject> _activeInstanceDict =
            new Dictionary<GameObject, PooledGameObject>();

        private readonly List<PooledGameObject> _idleInstances = new List<PooledGameObject>();

        private float _tickElapsedTime;

        public float DefaultTimeToRecycleObject { get; set; } = 0f;
        public float DefaultTimeToDestroyObject { get; set; } = 10f;

        /// <summary>
        /// 更新间隔
        /// </summary>
        public float TickInterval { get; set; } = 0.5f;

        public GameObjectPool()
        {
            ((IObjectPool)this).ObjectType = typeof(GameObject);
        }

        /// <summary>
        /// 对象池中对象的原始预制体
        /// </summary>
        public GameObject Original => _original;

        GameObject IGameObjectPool.Original
        {
            get => _original;
            set => _original = value;
        }

        /// <inheritdoc />
        public Transform Transform { get; set; }

        public override int ActiveCount => _activeInstanceDict.Count;
        public override int IdleCount => _idleInstances.Count;


        protected override object RentFromIdle()
        {
            PooledGameObject pooled;
            if (_idleInstances.Count > 0)
            {
                pooled = _idleInstances[^1];
                _idleInstances.RemoveAt(_idleInstances.Count - 1);
            }
            else
            {
                var instance = UnityEngine.Object.Instantiate(Original, Transform);
                pooled = new PooledGameObject(instance, this)
                {
                    ActiveLifetime = DefaultTimeToRecycleObject,
                    IdleLifetime = DefaultTimeToDestroyObject,
                };
            }

            _activeInstanceDict.Add(pooled.Target, pooled);

            pooled.State = PooledUnityObjectState.Avtive;
            pooled.ElapsedTime = 0f;

            var items = pooled.Target.GetComponents<IPoolItem>();

            if (items.Length > 0)
            {
                foreach (var receiver in items)
                {
                    receiver.Rent(this);
                }
            }

            return pooled.Target;
        }

        protected override bool ReleaseToIdle(object instance)
        {
            var gameObject = (GameObject)instance;

            if (!_activeInstanceDict.TryGetValue(gameObject, out var pooled))
            {
                return false; // 对象已经不在活跃列表中
            }

            pooled.State = PooledUnityObjectState.Idle;
            pooled.ElapsedTime = 0;

            gameObject.transform.SetParent(Transform, false);
            _activeInstanceDict.Remove(gameObject);
            _idleInstances.Add(pooled);

            var items = gameObject.GetComponents<IPoolItem>();
            if (items.Length > 0)
            {
                foreach (var receiver in items)
                {
                    receiver.Release(this);
                }
            }

            return true;
        }

        protected override bool RemoveFromActive(object instance)
        {
            var gameObject = (GameObject)instance;
            return _activeInstanceDict.Remove(gameObject);
        }

        protected override void ShrinkIdleObjectsToFitCapacity(int shrinkCount)
        {
            _idleInstances.RemoveRange(0, shrinkCount);
        }

        public IPooledGameObjectLifetimeAccessor GetLifetimeAccessor(GameObject instance)
        {
            var inst = GetInternalInstance(instance);
            return new PooledGameObjectLifetimeAccessor(
                () => inst.ActiveLifetime,
                f => inst.ActiveLifetime = f,
                () => inst.IdleLifetime,
                f => inst.IdleLifetime = f,
                () => inst.ElapsedTime,
                f => inst.ElapsedTime = f
            );
        }

        private PooledGameObject GetInternalInstance(GameObject instance)
        {
            var ret = _activeInstanceDict.GetValueOrDefault(instance);
            if (ret != null)
            {
                return ret;
            }

            ret = _idleInstances.Find(inst => inst.Target == instance);
            if (ret != null)
            {
                return ret;
            }

            throw new ArgumentException($"The specified instance '{instance.name}' is not managed by this object pool '{Name}'.");
        }

        void IGameObjectPool.Update(float deltaTime)
        {
            _tickElapsedTime += deltaTime;
            while (_tickElapsedTime >= TickInterval)
            {
                OnTick(TickInterval);
                _tickElapsedTime -= TickInterval;
            }
        }

        public new GameObject Rent()
        {
            return (GameObject)base.Rent();
        }


        private readonly List<PooledGameObject> _pendingRemoveInstances = new List<PooledGameObject>();
        private readonly List<PooledGameObject> _pendingRecycleInstances = new List<PooledGameObject>();
        private readonly List<PooledGameObject> _pendingDestroyInstances = new List<PooledGameObject>();

        /// <summary>
        /// 定时更新所有活跃对象的状态
        /// </summary>
        /// <param name="interval">更新间隔（秒）</param>
        protected virtual void OnTick(float interval)
        {
            foreach (var data in _activeInstanceDict.Values)
            {
                if (data.ActiveLifetime > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.ActiveLifetime)
                    {
                        _pendingRecycleInstances.Add(data);
                    }
                }
            }

            foreach (var data in _idleInstances)
            {
                if (data.IdleLifetime > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.IdleLifetime)
                    {
                        _pendingDestroyInstances.Add(data);
                    }
                }
            }

            DoPendingRecycleObjects();
            DoPendingDestroyObjects();
            DoPendingRemoveObjects();
        }

        private void DoPendingRecycleObjects()
        {
            if (_pendingRecycleInstances.Count > 0)
            {
                foreach (var data in _pendingRecycleInstances)
                {
                    // Simply call Release, we don't need to care about the return value here
                    Release(data.Target);
                }

                _pendingRecycleInstances.Clear();
            }
        }

        private void DoPendingDestroyObjects()
        {
            if (_pendingDestroyInstances.Count > 0)
            {
                foreach (var data in _pendingDestroyInstances)
                {
                    UnityEngine.Object.Destroy(data.Target);
                    _pendingRemoveInstances.Add(data);
                }

                _pendingDestroyInstances.Clear();
            }
        }

        private void DoPendingRemoveObjects()
        {
            if (_pendingRemoveInstances.Count > 0)
            {
                foreach (var data in _pendingRemoveInstances)
                {
                    if (data.State == PooledUnityObjectState.Avtive)
                    {
                        _activeInstanceDict.Remove(data.Target);
                    }
                    else
                    {
                        _idleInstances.Remove(data);
                    }
                }

                _pendingRemoveInstances.Clear();
            }
        }
    }
}

using UnityEngine;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Provides metadata about a pooled GameObject instance.
    /// </summary>
    public sealed class PooledGameObjectInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PooledGameObjectInfo"/> class.
        /// </summary>
        /// <param name="target">The target GameObject instance.</param>
        /// <param name="owningPool">The pool that owns this instance.</param>
        public PooledGameObjectInfo(GameObject target, IGameObjectPool owningPool)
        {
            Target = target;
            OwningPool = owningPool;
        }

        /// <summary>
        /// Gets the target GameObject instance.
        /// </summary>
        public GameObject Target { get; }

        /// <summary>
        /// Gets the pool that owns this instance.
        /// </summary>
        public IGameObjectPool OwningPool { get; }

        /// <summary>
        /// Gets or sets the maximum lifetime for the active state.
        /// Objects exceeding this time will be recycled back to the pool.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        public float ActiveLifetime { get; set; }

        /// <summary>
        /// Gets or sets the maximum lifetime for the idle state.
        /// Objects exceeding this time will be destroyed.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        public float IdleLifetime { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time since the last state change.
        /// <list type="bullet">
        ///   <item>When <see cref="State"/> is <see cref="PooledGameObjectState.Active"/>: represents active duration.</item>
        ///   <item>When <see cref="State"/> is <see cref="PooledGameObjectState.Idle"/>: represents idle duration.</item>
        /// </list>
        /// </summary>
        public float ElapsedTime { get; set; }

        /// <summary>
        /// Gets or sets the current state of the pooled GameObject.
        /// </summary>
        public PooledGameObjectState State { get; set; }
    }
}

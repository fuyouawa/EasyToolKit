using UnityEngine;

namespace EasyToolKit.Core.Implementations
{
    public class PoolManagerFactory : IPoolManagerFactory
    {
        /// <inheritdoc />
        public IObjectPoolManager CreateObjectPoolManager()
        {
            return new ObjectPoolManager();
        }

        /// <inheritdoc />
        public IGameObjectPoolManager CreateGameObjectPoolManager(Transform rootTransform = null)
        {
            return new GameObjectPoolManager(rootTransform);
        }
    }
}

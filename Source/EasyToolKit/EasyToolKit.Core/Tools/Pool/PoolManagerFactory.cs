using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Factory for creating pool manager instances.
    /// </summary>
    public static class PoolManagerFactory
    {
        private static IObjectPoolManager s_defaultObjectPoolManager;
        private static IGameObjectPoolManager s_defaultGameObjectPoolManager;

        /// <summary>
        /// Gets the default object pool manager instance.
        /// </summary>
        public static IObjectPoolManager DefaultObjectPoolManager {
            get
            {
                if (s_defaultObjectPoolManager == null)
                {
                    s_defaultObjectPoolManager = CreateObjectPoolManager();
                }
                return s_defaultObjectPoolManager;
            }
        }

        /// <summary>
        /// Gets the default GameObject pool manager instance.
        /// </summary>
        public static IGameObjectPoolManager DefaultGameObjectPoolManager
        {
            get
            {
                if (s_defaultGameObjectPoolManager == null)
                {
                    var gameObject = new GameObject(nameof(DefaultGameObjectPoolManager));
                    GameObject.DontDestroyOnLoad(gameObject);
                    s_defaultGameObjectPoolManager = CreateGameObjectPoolManager(gameObject.transform);
                }

                return s_defaultGameObjectPoolManager;
            }
        }

        /// <summary>
        /// Creates a new object pool manager.
        /// </summary>
        /// <returns>A new object pool manager instance.</returns>
        public static IObjectPoolManager CreateObjectPoolManager()
        {
            return new Implementations.ObjectPoolManager();
        }

        /// <summary>
        /// Creates a new GameObject pool manager.
        /// </summary>
        /// <param name="rootTransform">The root Transform for pooled GameObject hierarchy.</param>
        /// <returns>A new GameObject pool manager instance.</returns>
        public static IGameObjectPoolManager CreateGameObjectPoolManager(Transform rootTransform = null)
        {
            return new Implementations.GameObjectPoolManager(rootTransform);
        }
    }
}

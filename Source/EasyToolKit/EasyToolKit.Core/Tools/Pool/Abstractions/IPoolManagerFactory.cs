using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines a factory for creating pool manager instances.
    /// </summary>
    public interface IPoolManagerFactory
    {
        /// <summary>
        /// Creates a new C# object pool manager.
        /// </summary>
        /// <returns>A new object pool manager instance.</returns>
        IObjectPoolManager CreateObjectPoolManager();

        /// <summary>
        /// Creates a new GameObject pool manager.
        /// </summary>
        /// <param name="rootTransform">The Transform under which pool root nodes are organized.</param>
        /// <returns>A new GameObject pool manager instance.</returns>
        IGameObjectPoolManager CreateGameObjectPoolManager(Transform rootTransform = null);
    }
}

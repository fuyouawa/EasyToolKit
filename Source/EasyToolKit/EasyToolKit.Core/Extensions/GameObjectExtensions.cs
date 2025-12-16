using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides extension methods for Unity's GameObject to simplify common component operations
    /// and enhance developer productivity.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets an existing component of type T from the GameObject, or adds one if it doesn't exist.
        /// This is useful for ensuring a component is present without checking existence separately.
        /// </summary>
        /// <typeparam name="T">The type of component to get or add</typeparam>
        /// <param name="target">The target GameObject</param>
        /// <returns>The existing or newly added component of type T</returns>
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        /// <summary>
        /// Gets an existing component of the specified type from the GameObject, or adds one if it doesn't exist.
        /// </summary>
        /// <param name="target">The target GameObject</param>
        /// <param name="componentType">The type of component to get or add</param>
        /// <returns>The existing or newly added component</returns>
        public static Component GetOrAddComponent(this GameObject target, Type componentType)
        {
            if (!target.TryGetComponent(componentType, out var component))
            {
                component = target.AddComponent(componentType);
            }

            return component;
        }

        /// <summary>
        /// Determines whether the GameObject has a component of type T.
        /// </summary>
        /// <typeparam name="T">The type of component to check for</typeparam>
        /// <param name="target">The target GameObject</param>
        /// <returns>true if the GameObject has the component; otherwise, false</returns>
        public static bool HasComponent<T>(this GameObject target)
        {
            return target.TryGetComponent<T>(out _);
        }

        /// <summary>
        /// Determines whether the GameObject has a component of the specified type.
        /// </summary>
        /// <param name="target">The target GameObject</param>
        /// <param name="componentType">The type of component to check for</param>
        /// <returns>true if the GameObject has the component; otherwise, false</returns>
        public static bool HasComponent(this GameObject target, Type componentType)
        {
            return target.TryGetComponent(componentType, out _);
        }
    }
}

using EasyToolKit.ThirdParty.OdinSerializer;
using System;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Provides extension methods for EasyToolKit drawers.
    /// These extensions add utility functionality for drawer operations.
    /// </summary>
    public static class EasyDrawerExtensions
    {
        /// <summary>
        /// Gets a persistent context value for the drawer, stored across Unity sessions.
        /// The context is scoped to the drawer type, target type, and property path.
        /// </summary>
        /// <typeparam name="T">The type of the persistent value.</typeparam>
        /// <param name="drawer">The drawer instance.</param>
        /// <param name="key">The unique key for this persistent value.</param>
        /// <param name="defaultValue">The default value to return if no value is stored.</param>
        /// <returns>A LocalPersistentContext instance for managing the persistent value.</returns>
        public static LocalPersistentContext<T> GetPersistentContext<T>(this IEasyDrawer drawer, string key,
            T defaultValue = default)
        {
            var key1 = EasyDrawerUtility.GetKey(drawer);
            return PersistentContext.GetLocal(string.Join("+", key1, key), defaultValue);
        }
    }
}

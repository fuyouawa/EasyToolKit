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
            var key1 = TwoWaySerializationBinder.Default.BindToName(drawer.GetType());
            var key2 = TwoWaySerializationBinder.Default.BindToName(drawer.Property.Tree.TargetType);
            var key3 = drawer.Property.Path;

            return PersistentContext.GetLocal(string.Join("+", key1, key2, key3, key), defaultValue);
        }

        /// <summary>
        /// Gets the target type for attribute resolver operations.
        /// The target type depends on whether the attribute is applied to a type or a member.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <param name="drawer">The attribute drawer instance.</param>
        /// <returns>The target type for resolver operations.</returns>
        public static Type GetTargetTypeForResolver<TAttribute>(this EasyAttributeDrawer<TAttribute> drawer)
            where TAttribute : Attribute
        {
            return drawer.AttributeSource == AttributeSource.Type
                ? drawer.Property.ValueEntry.ValueType
                : drawer.Property.Parent.ValueEntry.ValueType;
        }

        /// <summary>
        /// Gets the target object for attribute resolver operations.
        /// The target object depends on whether the attribute is applied to a type or a member.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <param name="drawer">The attribute drawer instance.</param>
        /// <param name="targetIndex">The index of the target object in multi-object editing scenarios.</param>
        /// <returns>The target object for resolver operations.</returns>
        public static object GetTargetForResolver<TAttribute>(this EasyAttributeDrawer<TAttribute> drawer, int targetIndex = 0)
            where TAttribute : Attribute
        {
            return drawer.AttributeSource == AttributeSource.Type
                ? drawer.Property.ValueEntry.WeakValues[targetIndex]
                : drawer.Property.Parent.ValueEntry.WeakValues[targetIndex];
        }

        /// <summary>
        /// Gets the strongly-typed target object for attribute resolver operations.
        /// The target object depends on whether the attribute is applied to a type or a member.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <typeparam name="TValue">The type of the target value.</typeparam>
        /// <param name="drawer">The attribute drawer instance.</param>
        /// <param name="targetIndex">The index of the target object in multi-object editing scenarios.</param>
        /// <returns>The strongly-typed target object for resolver operations.</returns>
        public static TValue GetTargetForResolver<TAttribute, TValue>(this EasyAttributeDrawer<TAttribute, TValue> drawer, int targetIndex = 0)
            where TAttribute : Attribute
        {
            return drawer.AttributeSource == AttributeSource.Type
                ? drawer.ValueEntry.Values[targetIndex]
                : (drawer.Property.Parent.ValueEntry as IPropertyValueEntry<TValue>).Values[targetIndex];
        }
    }
}

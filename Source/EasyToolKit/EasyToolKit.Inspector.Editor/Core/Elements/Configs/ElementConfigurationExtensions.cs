using System;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for element configuration interfaces.
    /// Provides fluent API methods for configuring element properties.
    /// </summary>
    public static class ElementConfigurationExtensions
    {
        /// <summary>
        /// Sets the name of the element configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="name">The name to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithName<TConfiguration>(this TConfiguration configuration, string name)
            where TConfiguration : IElementConfiguration
        {
            configuration.Name = name;
            return configuration;
        }

        #region Value Configuration Extensions

        /// <summary>
        /// Sets the value type of a value configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The value configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="valueType">The value type to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithValueType<TConfiguration>(this TConfiguration configuration, Type valueType)
            where TConfiguration : IValueConfiguration
        {
            configuration.ValueType = valueType;
            return configuration;
        }

        /// <summary>
        /// Sets the value type of a value configuration using generic type parameter.
        /// </summary>
        /// <typeparam name="T">The value type to set.</typeparam>
        /// <typeparam name="TConfiguration">The value configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithValueType<T, TConfiguration>(this TConfiguration configuration)
            where TConfiguration : IValueConfiguration
        {
            configuration.ValueType = typeof(T);
            return configuration;
        }

        #endregion

        #region Property Configuration Extensions

        /// <summary>
        /// Configures the property to be rendered using Unity's built-in property drawer.
        /// </summary>
        /// <typeparam name="TConfiguration">The property configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="asUnityProperty">Whether to render the property using Unity's built-in property drawer.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration AsUnityProperty<TConfiguration>(this TConfiguration configuration, bool asUnityProperty = true)
            where TConfiguration : IPropertyConfiguration
        {
            configuration.AsUnityProperty = asUnityProperty;
            return configuration;
        }

        #endregion

        #region Group Configuration Extensions

        /// <summary>
        /// Sets the begin and end attribute types for a group configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="beginAttributeType">The attribute type that begins the group.</param>
        /// <param name="endAttributeType">The attribute type that ends the group.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttributes<TConfiguration>(this TConfiguration configuration,
            Type beginAttributeType, Type endAttributeType)
            where TConfiguration : IGroupConfiguration
        {
            configuration.BeginGroupAttributeType = beginAttributeType;
            configuration.EndGroupAttributeType = endAttributeType;
            return configuration;
        }

        /// <summary>
        /// Sets the begin and end attribute types for a group configuration using generic type parameters.
        /// </summary>
        /// <typeparam name="TBeginAttribute">The attribute type that begins the group.</typeparam>
        /// <typeparam name="TEndAttribute">The attribute type that ends the group.</typeparam>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttributes<TBeginAttribute, TEndAttribute, TConfiguration>(this TConfiguration configuration)
            where TBeginAttribute : Attribute
            where TEndAttribute : Attribute
            where TConfiguration : IGroupConfiguration
        {
            configuration.BeginGroupAttributeType = typeof(TBeginAttribute);
            configuration.EndGroupAttributeType = typeof(TEndAttribute);
            return configuration;
        }

        #endregion

        #region Method Configuration Extensions

        /// <summary>
        /// Sets the method info for a method configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The method configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="methodInfo">The method info to set.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithMethod<TConfiguration>(this TConfiguration configuration, MethodInfo methodInfo)
            where TConfiguration : IMethodConfiguration
        {
            configuration.MethodInfo = methodInfo;
            return configuration;
        }

        /// <summary>
        /// Sets the method info for a method configuration using expression tree.
        /// Note: This requires System.Linq.Expressions and additional implementation.
        /// </summary>
        /// <typeparam name="T">The declaring type.</typeparam>
        /// <typeparam name="TConfiguration">The method configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithMethod<T, TConfiguration>(this TConfiguration configuration, string methodName)
            where TConfiguration : IMethodConfiguration
        {
            var methodInfo = typeof(T).GetMethod(methodName);
            if (methodInfo == null)
                throw new ArgumentException($"Method '{methodName}' not found on type '{typeof(T).Name}'");

            configuration.MethodInfo = methodInfo;
            return configuration;
        }

        #endregion

        #region Collection Item Configuration Extensions

        /// <summary>
        /// Sets the collection item index for a collection item configuration.
        /// </summary>
        /// <typeparam name="TConfiguration">The collection item configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="index">The index of the item in the collection.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithIndex<TConfiguration>(this TConfiguration configuration, int index)
            where TConfiguration : ICollectionItemConfiguration
        {
            configuration.CollectionItemIndex = index;
            return configuration;
        }

        #endregion
    }
}

using System;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IObjectPoolBuilder{T}"/>.
    /// </summary>
    public static class ObjectPoolBuilderExtensions
    {
        /// <summary>
        /// Sets the factory function for creating new instances.
        /// </summary>
        public static IObjectPoolBuilder<T> WithFactory<T>(
            this IObjectPoolBuilder<T> builder, Func<T> factory)
            where T : class, new()
        {
            builder.Factory = factory;
            return builder;
        }
    }
}

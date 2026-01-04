namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IPoolBuilder"/>.
    /// </summary>
    public static class PoolBuilderExtensions
    {
        /// <summary>
        /// Sets the initial capacity of the pool.
        /// </summary>
        public static TBuilder WithInitialCapacity<TBuilder>(
            this TBuilder builder, int initialCapacity)
            where TBuilder : IPoolBuilder
        {
            builder.InitialCapacity = initialCapacity;
            return builder;
        }

        /// <summary>
        /// Sets the maximum capacity of the pool.
        /// </summary>
        public static TBuilder WithMaxCapacity<TBuilder>(
            this TBuilder builder, int maxCapacity)
            where TBuilder : IPoolBuilder
        {
            builder.MaxCapacity = maxCapacity;
            return builder;
        }

        /// <summary>
        /// Configures whether to call <see cref="IPoolItem"/> callbacks.
        /// </summary>
        public static TBuilder WithCallbacks<TBuilder>(
            this TBuilder builder, bool callCallbacks = true)
            where TBuilder : IPoolBuilder
        {
            builder.CallPoolItemCallbacks = callCallbacks;
            return builder;
        }
    }
}

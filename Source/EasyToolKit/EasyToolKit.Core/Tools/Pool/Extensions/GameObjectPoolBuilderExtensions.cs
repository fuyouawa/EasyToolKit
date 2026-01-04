namespace EasyToolKit.Core
{
    /// <summary>
    /// Extension methods for <see cref="IGameObjectPoolBuilder"/>.
    /// </summary>
    public static class GameObjectPoolBuilderExtensions
    {
        /// <summary>
        /// Sets the default maximum lifetime for active objects.
        /// </summary>
        public static IGameObjectPoolBuilder WithActiveLifetime(
            this IGameObjectPoolBuilder builder, float lifetime)
        {
            builder.DefaultActiveLifetime = lifetime;
            return builder;
        }

        /// <summary>
        /// Sets the default maximum lifetime for idle objects.
        /// </summary>
        public static IGameObjectPoolBuilder WithIdleLifetime(
            this IGameObjectPoolBuilder builder, float lifetime)
        {
            builder.DefaultIdleLifetime = lifetime;
            return builder;
        }

        /// <summary>
        /// Sets the interval between tick updates.
        /// </summary>
        public static IGameObjectPoolBuilder WithTickInterval(
            this IGameObjectPoolBuilder builder, float interval)
        {
            builder.TickInterval = interval;
            return builder;
        }

        /// <summary>
        /// Configures the pool for unlimited lifetime (no automatic recycling).
        /// </summary>
        public static IGameObjectPoolBuilder WithUnlimitedLifetime(
            this IGameObjectPoolBuilder builder)
        {
            builder.DefaultActiveLifetime = -1f;
            builder.DefaultIdleLifetime = -1f;
            return builder;
        }
    }
}

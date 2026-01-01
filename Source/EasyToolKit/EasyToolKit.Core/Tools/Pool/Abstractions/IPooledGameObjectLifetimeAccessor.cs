namespace EasyToolKit.Core
{
    /// <summary>
    /// Provides access to lifetime properties of a pooled GameObject.
    /// </summary>
    /// <remarks>
    /// This interface allows reading and modifying:
    /// <list type="bullet">
    ///   <item>Maximum lifetime in active state (objects exceeding this will be recycled).</item>
    ///   <item>Maximum lifetime in idle state (objects exceeding this will be destroyed).</item>
    ///   <item>Current elapsed time (represents time until recycling or destruction based on state).</item>
    /// </list>
    /// </remarks>
    public interface IPooledGameObjectLifetimeAccessor
    {
        /// <summary>
        /// Gets or sets the maximum lifetime for the active state.
        /// Objects exceeding this time will be recycled back to the pool.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float ActiveLifetime { get; set; }

        /// <summary>
        /// Gets or sets the maximum lifetime for the idle state.
        /// Objects exceeding this time will be destroyed.
        /// Values less than zero indicate unlimited lifetime.
        /// </summary>
        float IdleLifetime { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time since the last state change.
        /// <list type="bullet">
        ///   <item>When in active state: represents time until recycling.</item>
        ///   <item>When in idle state: represents time until destruction.</item>
        /// </list>
        /// </summary>
        float ElapsedTime { get; set; }
    }
}

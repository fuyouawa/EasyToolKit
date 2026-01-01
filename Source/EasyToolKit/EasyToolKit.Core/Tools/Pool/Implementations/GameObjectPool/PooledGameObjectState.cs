namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Defines the state of a pooled GameObject.
    /// </summary>
    public enum PooledGameObjectState
    {
        /// <summary>
        /// The GameObject is currently in use (rented from the pool).
        /// </summary>
        Active,

        /// <summary>
        /// The GameObject is idle (available for rent).
        /// </summary>
        Idle
    }
}

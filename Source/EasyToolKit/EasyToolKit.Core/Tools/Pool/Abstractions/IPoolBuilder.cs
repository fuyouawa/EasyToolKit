namespace EasyToolKit.Core
{
    /// <summary>
    /// Base interface for pool builders with common configuration properties.
    /// </summary>
    public interface IPoolBuilder
    {
        /// <summary>
        /// Gets or sets the initial capacity of the pool.
        /// </summary>
        int InitialCapacity { get; set; }

        /// <summary>
        /// Gets or sets the maximum capacity of the pool. Use -1 for unlimited.
        /// </summary>
        int MaxCapacity { get; set; }

        /// <summary>
        /// Gets or sets whether to call <see cref="IPoolItem"/> callbacks.
        /// </summary>
        bool CallPoolItemCallbacks { get; set; }
    }
}

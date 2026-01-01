namespace EasyToolKit.Core
{
    /// <summary>
    /// Base configuration interface for creating pool definitions.
    /// Provides mutable builder properties for pool configuration.
    /// </summary>
    public interface IPoolConfiguration
    {
        /// <summary>
        /// Gets or sets the initial capacity of the pool.
        /// The pool will preallocate this number of instances upon creation.
        /// </summary>
        int InitialCapacity { get; set; }

        /// <summary>
        /// Gets or sets the maximum capacity of the pool.
        /// Values less than zero indicate unlimited capacity.
        /// </summary>
        int MaxCapacity { get; set; }

        /// <summary>
        /// Gets or sets whether to call <see cref="IPoolItem"/> callbacks on pooled objects.
        /// </summary>
        bool CallPoolItemCallbacks { get; set; }
    }
}

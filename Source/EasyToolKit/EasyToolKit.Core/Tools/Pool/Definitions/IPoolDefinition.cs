namespace EasyToolKit.Core
{
    /// <summary>
    /// Defines the base interface for pool definitions.
    /// Contains immutable configuration data for pool initialization.
    /// </summary>
    public interface IPoolDefinition
    {
        /// <summary>
        /// Gets the initial capacity of the pool.
        /// The pool will preallocate this number of instances upon creation.
        /// </summary>
        int InitialCapacity { get; }

        /// <summary>
        /// Gets the maximum capacity of the pool.
        /// Values less than zero indicate unlimited capacity.
        /// </summary>
        int MaxCapacity { get; }

        /// <summary>
        /// Gets whether to call <see cref="IPoolItem"/> callbacks on pooled objects.
        /// </summary>
        bool CallPoolItemCallbacks { get; }
    }
}

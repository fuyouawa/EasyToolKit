using System;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Abstract base class for object pool implementations.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool.</typeparam>
    public abstract class PoolBase<T> : IPool<T>
    {
        private const int UnlimitedCapacity = -1;

        private readonly string _name;
        private int _capacity = UnlimitedCapacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolBase{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        protected PoolBase(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <inheritdoc />
        public string Name => _name;

        /// <inheritdoc />
        public abstract int ActiveCount { get; }

        /// <inheritdoc />
        public abstract int IdleCount { get; }

        /// <summary>
        /// Gets the total number of objects managed by this pool.
        /// </summary>
        public int TotalCount => ActiveCount + IdleCount;

        /// <inheritdoc />
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (_capacity == value)
                {
                    return;
                }

                if (value < UnlimitedCapacity)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"Capacity cannot be less than {UnlimitedCapacity}.");
                }

                if (value >= 0 && value < ActiveCount)
                {
                    throw new InvalidOperationException(
                        $"Capacity '{value}' cannot be less than active object count '{ActiveCount}'.");
                }

                _capacity = value;

                if (_capacity >= 0 && TotalCount > _capacity)
                {
                    ShrinkIdleObjectsToFitCapacity(TotalCount - _capacity);
                }
            }
        }

        /// <inheritdoc />
        public T Rent()
        {
            if (_capacity >= 0 && TotalCount >= _capacity && IdleCount == 0)
            {
                throw new InvalidOperationException(
                    $"No idle object available in pool '{Name}'. " +
                    $"Pool has reached capacity limit of {_capacity}.");
            }

            var instance = RentFromIdle();
            OnRent(instance);
            return instance;
        }

        /// <inheritdoc />
        public bool Release(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            bool released = ReleaseToIdle(instance);

            if (released)
            {
                OnRelease(instance);
            }

            return released;
        }

        /// <inheritdoc />
        public bool Remove(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return RemoveFromActive(instance);
        }

        /// <summary>
        /// Rents an instance from the idle collection or creates a new one.
        /// </summary>
        /// <returns>The rented instance.</returns>
        protected abstract T RentFromIdle();

        /// <summary>
        /// Releases an instance back to the idle collection.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><c>true</c> if successfully released; otherwise, <c>false</c>.</returns>
        protected abstract bool ReleaseToIdle(T instance);

        /// <summary>
        /// Removes an instance from the active collection.
        /// </summary>
        /// <param name="instance">The instance to remove.</param>
        /// <returns><c>true</c> if successfully removed; otherwise, <c>false</c>.</returns>
        protected abstract bool RemoveFromActive(T instance);

        /// <summary>
        /// Shrinks the idle collection to fit within the capacity constraint.
        /// </summary>
        /// <param name="shrinkCount">The number of idle instances to remove.</param>
        protected abstract void ShrinkIdleObjectsToFitCapacity(int shrinkCount);

        /// <summary>
        /// Called when an object is rented from the pool.
        /// Override to provide custom behavior.
        /// </summary>
        /// <param name="instance">The rented instance.</param>
        protected virtual void OnRent(T instance) { }

        /// <summary>
        /// Called when an object is released back to the pool.
        /// Override to provide custom behavior.
        /// </summary>
        /// <param name="instance">The released instance.</param>
        protected virtual void OnRelease(T instance) { }
    }
}

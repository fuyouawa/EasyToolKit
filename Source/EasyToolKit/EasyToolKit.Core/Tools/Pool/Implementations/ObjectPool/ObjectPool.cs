using System;
using System.Collections.Generic;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Implementation of a generic object pool for C# objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool.</typeparam>
    public class ObjectPool<T> : PoolBase<T>, IObjectPool<T> where T : class, new()
    {
        private readonly HashSet<T> _activeInstances = new HashSet<T>();
        private readonly Stack<T> _idleInstances = new Stack<T>();
        private readonly Func<T> _factory;
        private readonly bool _callPoolItemCallbacks;

        private Action<T> _onRent;
        private Action<T> _onRelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        /// <param name="definition">The definition for the pool.</param>
        /// <exception cref="ArgumentNullException">Thrown when definition is null.</exception>
        public ObjectPool(string name, IObjectPoolDefinition<T> definition) : base(name)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _factory = definition.Factory ?? (() => new T());
            _callPoolItemCallbacks = definition.CallPoolItemCallbacks;

            if (definition.InitialCapacity > 0)
            {
                PreallocateInstances(definition.InitialCapacity);
            }

            if (definition.MaxCapacity >= 0)
            {
                Capacity = definition.MaxCapacity;
            }
        }

        /// <inheritdoc />
        public override int ActiveCount => _activeInstances.Count;

        /// <inheritdoc />
        public override int IdleCount => _idleInstances.Count;

        /// <inheritdoc />
        public Type ObjectType => typeof(T);

        /// <inheritdoc />
        public void AddRentCallback(Action<T> callback)
        {
            _onRent += callback;
        }

        /// <inheritdoc />
        public void AddReleaseCallback(Action<T> callback)
        {
            _onRelease += callback;
        }

        /// <inheritdoc />
        protected override T RentFromIdle()
        {
            T instance;

            if (_idleInstances.Count > 0)
            {
                instance = _idleInstances.Pop();
            }
            else
            {
                instance = _factory();
            }

            _activeInstances.Add(instance);
            return instance;
        }

        /// <inheritdoc />
        protected override bool ReleaseToIdle(T instance)
        {
            if (instance == null)
            {
                return false;
            }

            if (!_activeInstances.Remove(instance))
            {
                return false; // Already idle or not from this pool
            }

            _idleInstances.Push(instance);
            return true;
        }

        /// <inheritdoc />
        protected override bool RemoveFromActive(T instance)
        {
            return _activeInstances.Remove(instance);
        }

        /// <inheritdoc />
        protected override void ShrinkIdleObjectsToFitCapacity(int shrinkCount)
        {
            for (int i = 0; i < shrinkCount && _idleInstances.Count > 0; i++)
            {
                _idleInstances.Pop();
            }
        }

        /// <inheritdoc />
        protected override void OnRent(T instance)
        {
            if (_callPoolItemCallbacks && instance is IPoolItem poolItem)
            {
                poolItem.Rent(this);
            }

            _onRent?.Invoke(instance);
        }

        /// <inheritdoc />
        protected override void OnRelease(T instance)
        {
            if (_callPoolItemCallbacks && instance is IPoolItem poolItem)
            {
                poolItem.Release(this);
            }

            _onRelease?.Invoke(instance);
        }

        private void PreallocateInstances(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _idleInstances.Push(_factory());
            }
        }
    }
}

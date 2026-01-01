using System;

namespace EasyToolKit.Core.Implementations
{
    /// <summary>
    /// Implementation of <see cref="IPooledGameObjectLifetimeAccessor"/> that provides
    /// access to pooled GameObject lifetime properties via getter and setter delegates.
    /// </summary>
    public sealed class PooledGameObjectLifetimeAccessor : IPooledGameObjectLifetimeAccessor
    {
        private readonly Func<float> _activeLifetimeGetter;
        private readonly Action<float> _activeLifetimeSetter;
        private readonly Func<float> _idleLifetimeGetter;
        private readonly Action<float> _idleLifetimeSetter;
        private readonly Func<float> _elapsedTimeGetter;
        private readonly Action<float> _elapsedTimeSetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledGameObjectLifetimeAccessor"/> class.
        /// </summary>
        /// <param name="activeLifetimeGetter">Function to get the active lifetime value.</param>
        /// <param name="activeLifetimeSetter">Action to set the active lifetime value.</param>
        /// <param name="idleLifetimeGetter">Function to get the idle lifetime value.</param>
        /// <param name="idleLifetimeSetter">Action to set the idle lifetime value.</param>
        /// <param name="elapsedTimeGetter">Function to get the elapsed time value.</param>
        /// <param name="elapsedTimeSetter">Action to set the elapsed time value.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any of the parameters is null.
        /// </exception>
        public PooledGameObjectLifetimeAccessor(
            Func<float> activeLifetimeGetter,
            Action<float> activeLifetimeSetter,
            Func<float> idleLifetimeGetter,
            Action<float> idleLifetimeSetter,
            Func<float> elapsedTimeGetter,
            Action<float> elapsedTimeSetter)
        {
            _activeLifetimeGetter = activeLifetimeGetter ?? throw new ArgumentNullException(nameof(activeLifetimeGetter));
            _activeLifetimeSetter = activeLifetimeSetter ?? throw new ArgumentNullException(nameof(activeLifetimeSetter));
            _idleLifetimeGetter = idleLifetimeGetter ?? throw new ArgumentNullException(nameof(idleLifetimeGetter));
            _idleLifetimeSetter = idleLifetimeSetter ?? throw new ArgumentNullException(nameof(idleLifetimeSetter));
            _elapsedTimeGetter = elapsedTimeGetter ?? throw new ArgumentNullException(nameof(elapsedTimeGetter));
            _elapsedTimeSetter = elapsedTimeSetter ?? throw new ArgumentNullException(nameof(elapsedTimeSetter));
        }

        /// <inheritdoc />
        public float ActiveLifetime
        {
            get => _activeLifetimeGetter();
            set => _activeLifetimeSetter(value);
        }

        /// <inheritdoc />
        public float IdleLifetime
        {
            get => _idleLifetimeGetter();
            set => _idleLifetimeSetter(value);
        }

        /// <inheritdoc />
        public float ElapsedTime
        {
            get => _elapsedTimeGetter();
            set => _elapsedTimeSetter(value);
        }
    }
}

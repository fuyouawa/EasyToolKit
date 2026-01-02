using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Specifies the timing of the value changed event.
    /// </summary>
    public enum ValueChangedTiming
    {
        /// <summary>
        /// The event is raised before the value is changed.
        /// </summary>
        Before,

        /// <summary>
        /// The event is raised after the value has been changed.
        /// </summary>
        After,
    }

    /// <summary>
    /// Provides data for the <see cref="IValueEntry{TValue}.BeforeValueChanged"/> and
    /// <see cref="IValueEntry{TValue}.AfterValueChanged"/> events.
    /// </summary>
    [MustDisposeResource]
    public class ValueChangedEventArgs : EventArgs, IPoolItem, IDisposable
    {
        /// <summary>
        /// Gets the zero-based index of the target object.
        /// </summary>
        public int TargetIndex { get; private set; }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        [CanBeNull] public object OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        [CanBeNull] public object NewValue { get; private set; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public ValueChangedTiming Timing { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ValueChangedEventArgs"/> class from the object pool.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="oldValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        /// <returns>A new or reused instance of <see cref="ValueChangedEventArgs"/>.</returns>
        public static ValueChangedEventArgs Create(int targetIndex, object oldValue, object newValue, ValueChangedTiming timing)
        {
            var args = EditorPoolUtility.Rent<ValueChangedEventArgs>();
            args.TargetIndex = targetIndex;
            args.OldValue = oldValue;
            args.NewValue = newValue;
            args.Timing = timing;
            return args;
        }

        /// <summary>
        /// Releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            EditorPoolUtility.Release(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
            TargetIndex = 0;
            OldValue = null;
            NewValue = null;
            Timing = default;
        }
    }
}

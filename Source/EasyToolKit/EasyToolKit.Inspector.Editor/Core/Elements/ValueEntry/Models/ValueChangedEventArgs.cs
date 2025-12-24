using System;

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
    public class ValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="targetIndex">The zero-based index of the target object.</param>
        /// <param name="oldValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="timing">The timing of the event (pre or post).</param>
        public ValueChangedEventArgs(int targetIndex, object oldValue, object newValue, ValueChangedTiming timing)
        {
            TargetIndex = targetIndex;
            OldValue = oldValue;
            NewValue = newValue;
            Timing = timing;
        }

        /// <summary>
        /// Gets the zero-based index of the target object.
        /// </summary>
        public int TargetIndex { get; }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object NewValue { get; }

        /// <summary>
        /// Gets the timing of the event (pre or post).
        /// </summary>
        public ValueChangedTiming Timing { get; }
    }
}

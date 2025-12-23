using System.Collections;

namespace EasyToolKit.Inspector.Editor
{
    public interface IValueEntry : IValueAccessor, IValueState, IValueChangeHandler
    {
        IValueElement OwnerElement { get; }
        bool IsReadOnly { get; }

        /// <summary>
        /// Updates the value entry with current values from the target objects.
        /// This refreshes the cached values from the actual property values.
        /// </summary>
        void Update();
    }

    public interface IValueEntry<TValue> : IValueEntry, IValueAccessor<TValue>
    {
    }
}

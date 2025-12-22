using System.Collections;

namespace EasyToolKit.Inspector.Editor
{
    public interface IValueEntry : IValueAccessor, IValueState, IValueChangeHandler
    {
        IValueElement Element { get; }
        bool IsReadOnly { get; }
    }

    public interface IValueEntry<TValue> : IValueEntry, IValueAccessor<TValue>
    {
    }
}

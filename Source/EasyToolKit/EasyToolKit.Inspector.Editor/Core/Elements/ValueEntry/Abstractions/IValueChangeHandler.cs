using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IValueChangeHandler
    {
        event EventHandler<ValueChangedEventArgs> PreValueChanged;
        event EventHandler<ValueChangedEventArgs> PostValueChanged;
        void ApplyChanges();
        void EnqueueChange(Action action);
    }
}

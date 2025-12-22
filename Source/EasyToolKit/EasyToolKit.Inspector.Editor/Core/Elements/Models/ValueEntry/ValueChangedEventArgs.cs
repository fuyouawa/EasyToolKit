using System;

namespace EasyToolKit.Inspector.Editor
{
    public class ValueChangedEventArgs : EventArgs
    {
        public int TargetIndex { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public ValueChangedEventArgs(int targetIndex, object oldValue, object newValue)
        {
            TargetIndex = targetIndex;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}

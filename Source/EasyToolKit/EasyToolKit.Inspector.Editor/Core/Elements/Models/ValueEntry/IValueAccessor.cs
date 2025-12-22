using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IValueAccessor
    {
        int TargetCount { get; }
        Type ValueType { get; }

        object WeakSmartValue { get; set; }

        object GetWeakValue(int targetIndex);
        void SetWeakValue(int targetIndex, object value);
    }

    public interface IValueAccessor<T> : IValueAccessor
    {
        T SmartValue { get; set; }

        T GetValue(int targetIndex);
        void SetValue(int targetIndex, T value);
    }
}

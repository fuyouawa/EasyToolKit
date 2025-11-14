using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueEntry : IDisposable
    {
        Type ValueType { get; }
        Type BaseValueType { get; }
        [CanBeNull] Type RuntimeValueType { get; }
        InspectorProperty Property { get; }

        int ValueCount { get; }
        object WeakSmartValue { get; set; }
        IPropertyValueCollection WeakValues { get; }

        event Action<int> OnValueChanged;

        bool IsConflicted();

        void Update();
        bool ApplyChanges();
    }

    public interface IPropertyValueEntry<TValue> : IPropertyValueEntry
    {
        TValue SmartValue { get; set; }
        IPropertyValueCollection<TValue> Values { get; }
    }
}
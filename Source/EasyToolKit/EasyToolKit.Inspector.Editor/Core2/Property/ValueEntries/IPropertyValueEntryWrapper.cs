namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a wrapper interface for property value entries.
    /// This interface is used to create specialized value entries that wrap existing entries.
    /// </summary>
    public interface IPropertyValueEntryWrapper : IPropertyValueEntry
    {
    }

    /// <summary>
    /// Represents a strongly-typed wrapper interface for property value entries.
    /// This generic interface provides type-safe wrapping of base value entries.
    /// </summary>
    /// <typeparam name="TValue">The specific type of the property value.</typeparam>
    /// <typeparam name="TBaseValue">The base type of the property value.</typeparam>
    public interface IPropertyValueEntryWrapper<TValue, TBaseValue> : IPropertyValueEntryWrapper, IPropertyValueEntry<TValue>
        where TValue : TBaseValue
    {
    }
}
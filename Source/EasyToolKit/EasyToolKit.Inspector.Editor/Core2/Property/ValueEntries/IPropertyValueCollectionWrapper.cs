namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a wrapper interface for property value collections.
    /// This interface is used to create specialized value collections that wrap existing collections.
    /// </summary>
    public interface IPropertyValueCollectionWrapper : IPropertyValueCollection
    {
    }

    /// <summary>
    /// Represents a strongly-typed wrapper interface for property value collections.
    /// This generic interface provides type-safe wrapping of base value collections.
    /// </summary>
    /// <typeparam name="TValue">The specific type of the property values.</typeparam>
    /// <typeparam name="TBaseValue">The base type of the property values.</typeparam>
    public interface IPropertyValueCollectionWrapper<TValue, TBaseValue> : IPropertyValueCollectionWrapper, IPropertyValueCollection<TValue>
        where TValue : TBaseValue
    {
    }
}
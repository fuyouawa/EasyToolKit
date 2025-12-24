namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property element interface representing properties on an object.
    /// Handles display and editing of <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    public interface IPropertyElement : IValueElement
    {
        /// <summary>
        /// Gets the property definition that describes this property.
        /// </summary>
        new IPropertyDefinition Definition { get; }

        new IValueElement LogicalParent { get; }
      }
}

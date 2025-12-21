namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property element interface representing fields or properties on an object.
    /// Uniformly handles display and editing of <see cref="System.Reflection.PropertyInfo"/> and <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public interface IPropertyElement : IValueElement
    {
        /// <summary>
        /// Gets the property definition that describes this property.
        /// </summary>
        new IPropertyDefinition Definition { get; }

        /// <summary>
        /// Gets the Unity‑style property path, equivalent to <see cref="IElement.Path"/> but formatted for Unity's <see cref="UnityEditor.SerializedProperty"/>.
        /// </summary>
        string UnityPath { get; }
    }
}

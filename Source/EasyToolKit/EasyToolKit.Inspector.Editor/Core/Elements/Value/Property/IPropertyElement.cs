namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Represents a property element in the inspector tree.
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

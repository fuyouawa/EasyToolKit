namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Field element interface representing fields on an object.
    /// Handles display and editing of <see cref="System.Reflection.FieldInfo"/>.
    /// </summary>
    public interface IFieldElement : IValueElement
    {
        /// <summary>
        /// Gets the field definition that describes this field.
        /// </summary>
        new IFieldDefinition Definition { get; }

        new IValueElement LogicalParent { get; }

        /// <summary>
        /// Gets the Unity‑style property path, equivalent to <see cref="IElement.Path"/> but formatted for Unity's <see cref="UnityEditor.SerializedProperty"/>.
        /// </summary>
        string UnityPath { get; }
    }
}

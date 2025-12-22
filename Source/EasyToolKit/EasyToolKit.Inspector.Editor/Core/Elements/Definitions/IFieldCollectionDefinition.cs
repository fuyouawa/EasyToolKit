namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Field collection definition interface that unifies <see cref="ICollectionDefinition"/> and <see cref="IFieldDefinition"/>.
    /// Represents collection fields on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public interface IFieldCollectionDefinition : ICollectionDefinition, IFieldDefinition
    {
    }
}
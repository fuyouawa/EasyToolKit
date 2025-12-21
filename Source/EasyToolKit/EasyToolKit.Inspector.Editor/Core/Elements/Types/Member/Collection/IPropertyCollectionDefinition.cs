using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Property collection definition interface that unifies <see cref="ICollectionDefinition"/> and <see cref="IPropertyDefinition"/>.
    /// Represents collection properties on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public interface IPropertyCollectionDefinition : ICollectionDefinition, IPropertyDefinition
    {
    }
}
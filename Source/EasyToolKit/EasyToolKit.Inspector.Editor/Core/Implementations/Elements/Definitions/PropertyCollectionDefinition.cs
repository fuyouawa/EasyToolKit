using System.Reflection;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property collection definition implementation that unifies <see cref="ICollectionDefinition"/> and <see cref="IPropertyDefinition"/>.
    /// Represents collection properties on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public sealed class PropertyCollectionDefinition : CollectionDefinition, IPropertyCollectionDefinition
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => PropertyInfo;
    }
}

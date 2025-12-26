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
        /// Initializes a new instance of the <see cref="PropertyCollectionDefinition"/> class.
        /// </summary>
        /// <param name="flags">The flags of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered (can be accessed by index).</param>
        public PropertyCollectionDefinition(ElementFlags flags, string name, PropertyInfo propertyInfo, System.Type itemType, bool isOrdered)
            : base(flags, name, propertyInfo.PropertyType, itemType, isOrdered)
        {
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        public MemberInfo MemberInfo => PropertyInfo;
    }
}

using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property collection element implementation representing collection properties on an object.
    /// Handles display and editing of collection-based <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    public class PropertyCollectionElement : CollectionElement, IPropertyCollectionElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollectionElement"/> class.
        /// </summary>
        /// <param name="definition">The property collection definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public PropertyCollectionElement(
            [NotNull] IPropertyCollectionDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the property collection definition that describes this property collection.
        /// </summary>
        public new IPropertyCollectionDefinition Definition => (IPropertyCollectionDefinition)base.Definition;

        IPropertyDefinition IPropertyElement.Definition => Definition;

        public new IValueElement LogicalParent => (IValueElement)base.LogicalParent;
    }
}

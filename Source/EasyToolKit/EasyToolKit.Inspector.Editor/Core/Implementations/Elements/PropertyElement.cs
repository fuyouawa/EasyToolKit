using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property element implementation representing properties on an object.
    /// Handles display and editing of <see cref="System.Reflection.PropertyInfo"/>.
    /// </summary>
    public class PropertyElement : ValueElement, IPropertyElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyElement"/> class.
        /// </summary>
        /// <param name="definition">The property definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <param name="logicalParent">The logical parent element in the code structure.</param>
        public PropertyElement(
            [NotNull] IPropertyDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IValueElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        /// <summary>
        /// Gets the property definition that describes this property.
        /// </summary>
        public new IPropertyDefinition Definition => (IPropertyDefinition)base.Definition;

        public IValueElement LogicalParent => (IValueElement)base.LogicalParent;
    }
}

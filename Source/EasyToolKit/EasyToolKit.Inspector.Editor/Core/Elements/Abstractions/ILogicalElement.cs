using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public interface ILogicalElement : IElement
    {
        /// <summary>
        /// Gets the logical parent element that owns this element in the code structure.
        /// This represents the static parent relationship defined by the element's definition and does not change
        /// during runtime modifications or tree restructuring operations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property can be <c>null</c> for root elements, group elements,
        /// or custom elements created dynamically by users.
        /// </para>
        /// </remarks>
        [CanBeNull] ILogicalElement LogicalParent { get; }

        /// <summary>
        /// Gets the child elements defined by the code structure.
        /// These represent the static hierarchy determined by the element's definition and are immutable.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Logical children are created during initialization based on the <see cref="IStructureResolver"/>
        /// and remain constant throughout the element's lifecycle.
        /// </para>
        /// <para>
        /// This property can be <c>null</c> for elements that cannot have children.
        /// </para>
        /// </remarks>
        [CanBeNull] IReadOnlyElementList<ILogicalElement> LogicalChildren { get; }
    }
}

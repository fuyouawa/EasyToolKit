using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class CollectionElement : ValueElement, ICollectionElement
    {
        private IReadOnlyElementListWrapper<ICollectionItemElement, IElement> _logicalChildrenWrapper;

        public CollectionElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        public ICollectionDefinition Definition => (ICollectionDefinition)base.Definition;
        public IReadOnlyElementList<ICollectionItemElement> LogicalChildren => _logicalChildrenWrapper;
        public ICollectionEntry ValueEntry => (ICollectionEntry)base.ValueEntry;

        protected override bool CanHaveChildren()
        {
            return true;
        }

        protected override void OnCreatedChildren()
        {
            base.OnCreatedChildren();
            _logicalChildrenWrapper = new ReadOnlyElementListWrapper<ICollectionItemElement, IElement>(base.LogicalChildren!);
        }
    }
}

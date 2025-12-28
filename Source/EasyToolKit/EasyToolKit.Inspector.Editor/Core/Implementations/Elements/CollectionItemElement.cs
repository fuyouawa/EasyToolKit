using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class CollectionItemElement : ValueElement, ICollectionItemElement
    {
        public CollectionItemElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] ILogicalElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        public ICollectionItemDefinition Definition => (ICollectionItemDefinition)base.Definition;
        public ICollectionElement LogicalParent => (ICollectionElement)base.LogicalParent;
    }
}

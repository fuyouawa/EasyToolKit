using System;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class CollectionElement : ValueElement, ICollectionElement
    {
        public CollectionElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] IElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        public ICollectionDefinition Definition => (ICollectionDefinition)base.Definition;

        public IReadOnlyElementList<ICollectionItemElement> LogicalChildren =>
            ((IReadOnlyElementListBoxedWrapper<IElement, ICollectionItemElement>)base.LogicalChildren)!.DerivedList;

        public ICollectionEntry BaseValueEntry  => (ICollectionEntry)base.BaseValueEntry;
        public ICollectionEntry ValueEntry => (ICollectionEntry)base.ValueEntry;

        protected override bool CanHaveChildren()
        {
            return true;
        }

        protected override void OnUpdate(bool forceUpdate)
        {
            base.OnUpdate(forceUpdate);

            var minimumItemCount = ValueEntry.GetMinimumItemCount();
            if (minimumItemCount > LogicalChildren.Count)
            {
                Refresh();
            }
        }

        protected override IReadOnlyElementList<IElement> CreateLogicalChildren()
        {
            var baseLogicalChildren = base.CreateLogicalChildren();
            var wrapper = new ReadOnlyElementListWrapper<ICollectionItemElement, IElement>(baseLogicalChildren);
            return new ReadOnlyElementListBoxedWrapper<IElement, ICollectionItemElement>(wrapper);
        }

        protected override IValueEntry CreateBaseValueEntry()
        {
            var valueEntryType = typeof(CollectionEntry<,>).MakeGenericType(Definition.ValueType, Definition.ItemType);
            return valueEntryType.CreateInstance<IValueEntry>(this);
        }

        protected override IValueEntry CreateWrapperValueEntry()
        {
            var valueEntryType = typeof(CollectionEntryWrapper<,,,>).MakeGenericType(
                BaseValueEntry.RuntimeValueType,
                BaseValueEntry.RuntimeItemType,
                BaseValueEntry.ValueType,
                BaseValueEntry.ItemType);
            return valueEntryType.CreateInstance<IValueEntry>(BaseValueEntry);
        }

        protected override void PostProcessBaseValueEntry(IValueEntry baseValueEntry)
        {
            base.PostProcessBaseValueEntry(baseValueEntry);
            var collectionEntry = (ICollectionEntry)baseValueEntry;
            collectionEntry.AfterCollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}

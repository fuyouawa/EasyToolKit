using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(1.0)]
    public abstract class CollectionStructureResolverBase<TCollection> : ValueStructureResolverBase<TCollection>, ICollectionStructureResolver
    {
        private ICollectionItemDefinition[] _definitions;

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public abstract Type ItemType { get; }

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Flags.IsCollection();
        }

        protected override void Initialize()
        {
            var count = CalculateChildCount();
            _definitions = new ICollectionItemDefinition[count];
            for (int i = 0; i < count; i++)
            {
                _definitions[i] = InspectorElements.Configurator.CollectionItem()
                    .WithItemIndex(i)
                    .WithValueType(ItemType)
                    .WithName($"Array.data[{i}]")
                    .CreateDefinition();
            }
        }

        protected override IElementDefinition[] GetChildrenDefinitions()
        {
            return _definitions;
        }

        protected abstract int CalculateChildCount();
    }
}

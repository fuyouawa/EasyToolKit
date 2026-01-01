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
            return element.Definition.Roles.IsCollection();
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

        /// <summary>
        /// Clears the cached collection item definitions when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _definitions = null;
        }

        protected abstract int CalculateChildCount();
    }
}

using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class DelayedElementList<TElement> : ElementList<TElement>
        where TElement : IElement
    {
        public DelayedElementList([NotNull] IElement ownerElement) : base(ownerElement)
        {
        }

        public DelayedElementList([NotNull] IElement ownerElement, [CanBeNull] IEnumerable<TElement> initialElements) : base(ownerElement, initialElements)
        {
        }

        public override void Insert(int index, TElement element)
        {
            OwnerElement.SharedContext.Tree.QueueCallbackUntilRepaint(() => base.Insert(index, element));
        }

        public override void RemoveAt(int index)
        {
            OwnerElement.SharedContext.Tree.QueueCallbackUntilRepaint(() => base.RemoveAt(index));
        }

        public override void Clear()
        {
            OwnerElement.SharedContext.Tree.QueueCallbackUntilRepaint(() => base.Clear());
        }
    }
}

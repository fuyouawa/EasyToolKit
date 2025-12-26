using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor.Implementations
{
    public class RequestedElementList<TElement> : ElementList<TElement>
        where TElement : IElement
    {
        public RequestedElementList([NotNull] IElement ownerElement) : base(ownerElement)
        {
        }

        public RequestedElementList([NotNull] IElement ownerElement, [CanBeNull] IEnumerable<TElement> initialElements) : base(ownerElement, initialElements)
        {
        }

        public override void Insert(int index, TElement element)
        {
            OwnerElement.Request(() => base.Insert(index, element));
        }

        public override void RemoveAt(int index)
        {
            OwnerElement.Request(() => base.RemoveAt(index));
        }

        public override void Clear()
        {
            OwnerElement.Request(() => base.Clear());
        }
    }
}

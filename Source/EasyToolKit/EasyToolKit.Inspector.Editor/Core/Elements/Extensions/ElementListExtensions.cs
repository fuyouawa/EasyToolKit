namespace EasyToolKit.Inspector.Editor
{
    public static class ElementListExtensions
    {
        public static void Add<TElement>(this IElementList<TElement> elementList, TElement element)
            where TElement : IElement
        {
            elementList.Insert(elementList.Count, element);
        }

        public static bool TryRemove<TElement>(this IElementList<TElement> elementList, TElement element)
            where TElement : IElement
        {
            var index = elementList.IndexOf(element);
            if (index == -1)
            {
                return false;
            }

            elementList.RemoveAt(index);
            return true;
        }

        public static bool Contains<TElement>(this IElementList<TElement> elementList, TElement element)
            where TElement : IElement
        {
            return elementList.IndexOf(element) != -1;
        }
    }
}

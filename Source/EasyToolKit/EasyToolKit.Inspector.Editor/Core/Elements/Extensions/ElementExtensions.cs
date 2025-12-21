namespace EasyToolKit.Inspector.Editor
{
    public static class ElementExtensions
    {
        public static IValueElement AsValue(this IElement element)
        {
            return element as IValueElement;
        }
    }
}

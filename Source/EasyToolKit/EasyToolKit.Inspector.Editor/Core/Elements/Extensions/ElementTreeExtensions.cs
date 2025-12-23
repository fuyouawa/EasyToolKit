namespace EasyToolKit.Inspector.Editor
{
    public static class ElementTreeExtensions
    {
        public static void Draw(this IElementTree elementTree)
        {
            elementTree.BeginDraw();
            elementTree.DrawElements();
            elementTree.EndDraw();
        }
    }
}

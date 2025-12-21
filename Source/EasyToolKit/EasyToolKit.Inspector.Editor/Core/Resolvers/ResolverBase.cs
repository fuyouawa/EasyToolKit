namespace EasyToolKit.Inspector.Editor
{
    public class ResolverBase : HandlerBase, IResolver
    {
        protected override bool CanHandle(IElement element)
        {
            return CanResolve(element);
        }

        protected virtual bool CanResolve(IElement element) => true;
    }
}

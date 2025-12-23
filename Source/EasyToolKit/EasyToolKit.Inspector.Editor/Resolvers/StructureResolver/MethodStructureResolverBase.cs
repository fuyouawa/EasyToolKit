namespace EasyToolKit.Inspector.Editor
{
    public abstract class MethodStructureResolverBase : StructureResolverBase
    {
        new public IMethodElement Element => base.Element as IMethodElement;

        protected override bool CanResolve(IElement element)
        {
            if (element is IMethodElement methodElement)
            {
                return CanResolveElement(methodElement);
            }

            return false;
        }

        protected virtual bool CanResolveElement(IMethodElement element)
        {
            return true;
        }
    }
}

namespace EasyToolKit.Inspector.Editor
{
    public abstract class PropertyOperationResolverBase : ResolverBase, IPropertyOperationResolver
    {
        private bool _isInitialized;

        new public IValueElement Element => base.Element as IValueElement;

        protected abstract IPropertyOperation GetOperation();

        protected virtual void Initialize()
        {
        }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        IPropertyOperation IPropertyOperationResolver.GetOperation()
        {
            EnsureInitialize();
            return GetOperation();
        }

        protected override bool CanResolve(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                return CanResolveElement(valueElement);
            }

            return false;
        }

        protected virtual bool CanResolveElement(IValueElement element)
        {
            return true;
        }
    }
}

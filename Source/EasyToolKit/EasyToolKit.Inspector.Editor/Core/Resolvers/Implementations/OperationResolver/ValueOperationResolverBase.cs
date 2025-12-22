namespace EasyToolKit.Inspector.Editor
{
    public abstract class ValueOperationResolverBase : ResolverBase, IValueOperationResolver
    {
        private bool _isInitialized;

        new public IValueElement Element => base.Element as IValueElement;

        protected abstract IValueOperation GetOperation();

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

        IValueOperation IValueOperationResolver.GetOperation()
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

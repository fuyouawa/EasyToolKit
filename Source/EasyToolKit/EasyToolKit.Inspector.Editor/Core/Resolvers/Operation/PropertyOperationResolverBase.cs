namespace EasyToolKit.Inspector.Editor
{
    public abstract class PropertyOperationResolverBase : ResolverBase, IPropertyOperationResolver
    {
        private bool _isInitialized;

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
    }
}

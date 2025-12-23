namespace EasyToolKit.Inspector.Editor
{
    public abstract class StructureResolverBase : ResolverBase, IStructureResolver
    {
        private bool _isInitialized;

        protected virtual void Initialize()
        {
        }

        protected abstract IElementDefinition[] GetChildrenDefinitions();

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        IElementDefinition[] IStructureResolver.GetChildrenDefinitions()
        {
            EnsureInitialize();
            return GetChildrenDefinitions();
        }
    }
}

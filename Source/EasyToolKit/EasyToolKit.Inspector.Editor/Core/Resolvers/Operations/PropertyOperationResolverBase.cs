namespace EasyToolKit.Inspector.Editor
{
    public abstract class PropertyOperationResolverBase : InspectorResolverBase, IPropertyOperationResolver
    {
        public abstract IPropertyOperation GetOperation();
    }
}

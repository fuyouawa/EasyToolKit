namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyResolverLocator
    {
        IPropertyResolver GetResolver(InspectorProperty property);
    }

    public abstract class PropertyResolverLocator : IPropertyResolverLocator
    {
        public abstract IPropertyResolver GetResolver(InspectorProperty property);
    }
}
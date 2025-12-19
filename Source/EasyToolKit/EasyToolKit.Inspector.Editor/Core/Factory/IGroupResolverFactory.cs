namespace EasyToolKit.Inspector.Editor
{
    public interface IGroupResolverFactory
    {
        IGroupResolver CreateResolver(InspectorProperty property);
    }
}
namespace EasyToolKit.Inspector.Editor
{
    public class InspectorResolverBase : InspectorElementBase
    {
        protected override bool CanHandle(InspectorProperty property)
        {
            return CanResolve(property);
        }

        protected virtual bool CanResolve(InspectorProperty property) => true;
    }
}

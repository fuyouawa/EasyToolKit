using System;

namespace EasyToolKit.Inspector.Editor
{
    [ResolverPriority(100000.0)]
    public class RootPropertyOperationResolver : PropertyOperationResolverBase
    {
        private IPropertyOperation _operation;

        protected override bool CanResolve(InspectorProperty property)
        {
            return property.Info.IsLogicRoot;
        }

        protected override void Initialize()
        {
            _operation = new GenericPropertyOperation(
                null, Property.ValueEntry.ValueType,
                (ref object index) => Property.Tree.Targets[(int)index],
                (ref object index, object value) => throw new InvalidOperationException("Cannot set logic root"));
        }

        protected override IPropertyOperation GetOperation()
        {
            return _operation;
        }
    }
}
